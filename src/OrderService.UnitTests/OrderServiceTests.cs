extern alias ordersvc;

using Bogus;
using CoreShared;
using DeepEqual.Syntax;
using MockQueryable.Moq;
using Moq;
using ordersvc::OrderService.Contracts.Dtos;
using ordersvc::OrderService.Database;
using ordersvc::OrderService.Database.Entities;
using ordersvc::ProductService;

namespace OrderService.UnitTests;

[TestClass]
public sealed class OrderServiceTests
{
    private readonly List<OrderEntity> _orderEntities;
    private readonly List<PaymentEntity> _paymentEntities;
    
    private readonly AppDbContext _dbContext;
    private readonly Mock<AppDbContext> _dbContextMock;
    
    public OrderServiceTests()
    {
        (_orderEntities, _paymentEntities) = Helpers.GetDbEntities();
        
        _dbContextMock = new Mock<AppDbContext>();

        var ordersDbSet = _orderEntities.BuildMockDbSet();
        var paymentsDbSet = _paymentEntities.BuildMockDbSet();

        _dbContextMock.Setup(x => x.Orders).Returns(ordersDbSet.Object);
        _dbContextMock.Setup(x => x.Payments).Returns(paymentsDbSet.Object);
        
        _dbContext = _dbContextMock.Object;
    }
    
    [TestMethod]
    public async Task CreateOrder_WhenOrderAlreadyCreated_ThrowsProductSoldException()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);
        
        var order = _orderEntities[0];
        var productId = order.ProductId;
        var userId = order.UserId;
        
        // Arrange
        var exception = await Assert.ThrowsExceptionAsync<ProblemException>(async () => await orderService.CreateOrderAsync(productId, userId));

        // Assert
        Assert.AreEqual(ExceptionMessages.ProductSold, exception.Error);
    }

    [TestMethod]
    public async Task CreateOrder_WhenProductDoesNotExists_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();

        productClientMock
            .Setup(x => x.GetProductAsync(It.IsAny<GetProductReq>(), null, null, CancellationToken.None))
            .Returns(GrpcCallHelpers.CreateAsyncUnaryCall<ProductDto?>(null));
        
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);
        
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        // Arrange
        var response = await orderService.CreateOrderAsync(productId, userId);
        
        // Assert
        Assert.IsNull(response);
    }

    [TestMethod]
    public async Task CreateOrder_WhenCorrectRequest_CreatesNewOrder()
    {
        // Act
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var faker = new Faker();
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();

        productClientMock
            .Setup(x => x.GetProductAsync(It.IsAny<GetProductReq>(), null, null, CancellationToken.None))
            .Returns(GrpcCallHelpers.CreateAsyncUnaryCall<ProductDto?>(new ProductDto
            {
                Id = productId.ToString(),
                Name = faker.Commerce.ProductName(),
                Description = faker.Commerce.ProductDescription(),
                PriceCents = faker.Random.Long(200, 15000000)
            }));
        
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);
        
        // Arrange
        var orderDto = await orderService.CreateOrderAsync(productId, userId);
        
        // Assert
        Assert.IsNotNull(orderDto);
        Assert.IsNotNull(orderDto.Id);
        Assert.AreEqual(productId, orderDto.ProductId);
        Assert.AreEqual(false, orderDto.Paid);
        
        _dbContextMock.Verify(x => x.Orders.Add(It.IsAny<OrderEntity>()), Times.Once());
        _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
    }

    [TestMethod]
    public async Task GetOrder_WhenOrderDoesNotExists_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);
        
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        // Arrange
        var response = await orderService.GetOrderAsync(productId, userId);
        
        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task GetOrder_WhenOrderExists_ButOtherUser_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);
        
        var order = _orderEntities[0];
        var productId = order.ProductId;
        var userId = Guid.NewGuid();
        
        // Arrange
        var response = await orderService.GetOrderAsync(productId, userId);
        
        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task GetOrder_WhenOrderExists_ReturnsOrder()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);
        
        var order = _orderEntities[0];
        var orderId = order.Id;
        var userId = order.UserId;
        
        // Arrange
        var orderDto = await orderService.GetOrderAsync(orderId, userId);
        
        // Assert
        Assert.IsNotNull(orderDto);
        Assert.AreEqual(order.Payments.Any(x => x.Paid), orderDto.Paid);
        Assert.AreEqual(orderId, orderDto.Id);
        Assert.AreEqual(order.ProductId, orderDto.ProductId);
    }
    
    [TestMethod]
    public async Task GetOrders_WhenOrdersExists_ButOtherUser_ReturnsEmptyList()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);

        var userId = Guid.NewGuid();
        
        // Arrange
        var ordersDto = await orderService.GetOrdersAsync(userId);
        
        // Assert
        Assert.IsNotNull(ordersDto);
        Assert.IsEmpty(ordersDto);
    }
    
    [TestMethod]
    public async Task GetOrders_WhenOrdersExists_ReturnsOrders()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var orderService = new ordersvc::OrderService.Services.OrderService(_dbContext, productClientMock.Object);

        var order = _orderEntities[0];
        var userId = order.UserId;
        
        // Arrange
        var ordersDto = await orderService.GetOrdersAsync(userId);
        
        // Assert
        Assert.IsNotNull(ordersDto);

        var validOrders = _orderEntities
            .Where(x => x.UserId == userId)
            .Select(orderEntity => new OrderDto
            {
                Id = orderEntity.Id,
                ProductId = orderEntity.ProductId,
                Paid = orderEntity.Payments.Any(x => x.Paid),
                PaymentExpirationDate = orderEntity.Payments
                    .Where(x => x.OrderId == orderEntity.Id)
                    .Any(x => x.Paid)
                        ? null
                        : orderEntity.Payments
                            .FirstOrDefault(x => !x.Paid && x.ExpiresAt > DateTime.UtcNow)?.ExpiresAt
            });

        validOrders.ShouldDeepEqual(ordersDto);
    }
}