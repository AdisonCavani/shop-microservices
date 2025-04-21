extern alias ordersvc;

using Bogus;
using CoreShared;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using ordersvc::OrderService.Contracts.Dtos;
using ordersvc::OrderService.Database;
using ordersvc::OrderService.Database.Entities;
using ordersvc::ProductService;
using ProtobufSpec.Events;

namespace OrderService.UnitTests;

[TestClass]
public sealed class PaymentServiceTests
{
    private readonly List<OrderEntity> _orderEntities;
    private readonly List<PaymentEntity> _paymentEntities;
    
    private readonly AppDbContext _dbContext;
    private readonly Mock<AppDbContext> _dbContextMock;
    
    private readonly IBus _bus;
    private readonly Mock<IBus> _busMock;
    
    public PaymentServiceTests()
    {
        (_orderEntities, _paymentEntities) = Helpers.GetDbEntities();
        
        _dbContextMock = new Mock<AppDbContext>();

        var ordersDbSet = _orderEntities.BuildMockDbSet();
        var paymentsDbSet = _paymentEntities.BuildMockDbSet();

        _dbContextMock.Setup(x => x.Orders).Returns(ordersDbSet.Object);
        _dbContextMock.Setup(x => x.Payments).Returns(paymentsDbSet.Object);
        
        _dbContext = _dbContextMock.Object;
        
        _busMock = new Mock<IBus>();
        _bus = _busMock.Object;
    }
    
    [TestMethod]
    public async Task CreatePayment_WhenOrderDoesNotExists_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);

        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = new Faker().Person.Email;
        
        // Arrange
        var response = await paymentService.CreatePaymentAsync(orderId, userId, userEmail);

        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task CreatePayment_WhenOrderAlreadyPaid_ThrowsOrderAlreadyPaidException()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);

        var payment = _paymentEntities.First(x => x.Paid);
        var orderId = payment.OrderId;
        var userId = payment.Order.UserId;
        var userEmail = new Faker().Person.Email;
        
        // Arrange
        var exception = await Assert.ThrowsExceptionAsync<ProblemException>(async () => await paymentService.CreatePaymentAsync(orderId, userId, userEmail));

        // Assert
        Assert.AreEqual(ExceptionMessages.OrderAlreadyPaid, exception.Error);
    }
    
    [TestMethod]
    public async Task CreatePayment_WhenValidPaymentExists_ThrowsPaymentAlreadyCreatedException()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);

        var payment = _paymentEntities.First(x => !x.Paid && x.ExpiresAt > DateTime.UtcNow);
        var orderId = payment.OrderId;
        var userId = payment.Order.UserId;
        var userEmail = new Faker().Person.Email;
        
        // Arrange
        var exception = await Assert.ThrowsExceptionAsync<ProblemException>(async () => await paymentService.CreatePaymentAsync(orderId, userId, userEmail));

        // Assert
        Assert.AreEqual(ExceptionMessages.PaymentAlreadyCreated, exception.Error);
    }
    
    [TestMethod]
    public async Task CreatePayment_WhenProductDoesNotExists_ThrowsProductLostException()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        
        productClientMock
            .Setup(x => x.GetProductAsync(It.IsAny<GetProductReq>(), null, null, CancellationToken.None))
            .Returns(GrpcCallHelpers.CreateAsyncUnaryCall<ProductDto?>(null));
        
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);

        var order = _orderEntities
            .First(x =>
                !x.Payments.Any(p => p.Paid) &&
                (
                    !x.Payments.Any() ||
                    x.Payments.All(p => p.ExpiresAt < DateTime.UtcNow)
                )
            );
        var orderId = order.Id;
        var userId = order.UserId;
        var userEmail = new Faker().Person.Email;
        
        // Arrange
        var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await paymentService.CreatePaymentAsync(orderId, userId, userEmail));

        // Assert
        Assert.AreEqual(ExceptionMessages.ProductLost, exception.Message);
    }

    [TestMethod]
    public async Task GetPayment_WhenOrderDoesNotExists_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);
        
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Arrange
        var response = await paymentService.GetPaymentAsync(orderId, userId);
        
        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task GetPayment_WhenOrderExists_ButOtherUser_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);
        
        var order = _orderEntities.First();
        var orderId = order.Id;
        var userId = Guid.NewGuid();

        // Arrange
        var response = await paymentService.GetPaymentAsync(orderId, userId);
        
        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task GetPayment_WhenNoPayments_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);
        
        var order = _orderEntities.First(x => !x.Payments.Any());
        var orderId = order.Id;
        var userId = order.UserId;

        // Arrange
        var response = await paymentService.GetPaymentAsync(orderId, userId);
        
        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task GetPayment_WhenAllPaymentsExpired_ReturnsNull()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);
        
        var order = _orderEntities.First(x => x.Payments.Any() && x.Payments.All(p => !p.Paid && p.ExpiresAt < DateTime.UtcNow));
        var orderId = order.Id;
        var userId = order.UserId;

        // Arrange
        var response = await paymentService.GetPaymentAsync(orderId, userId);
        
        // Assert
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task GetPayment_WhenOrderPaid_ReturnsPayment()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);
        
        var order = _orderEntities.First(x => x.Payments.Any(y => y.Paid));
        var orderId = order.Id;
        var userId = order.UserId;

        // Arrange
        var paymentDto = await paymentService.GetPaymentAsync(orderId, userId);
        
        // Assert
        Assert.IsNotNull(paymentDto);
        Assert.AreEqual(true, paymentDto.Paid);
        Assert.IsNull(paymentDto.PaymentUrl);
        Assert.IsNull(paymentDto.ExpirationDate);
    }
    
    [TestMethod]
    public async Task PaymentReceived_WhenPaymentDoesNotExists_ThrowsPaymentLostException()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);

        var message = new PaymentSucceededEvent
        {
            PaymentId = Guid.NewGuid()
        };
        
        // Arrange
        var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await paymentService.PaymentReceivedAsync(message));

        // Assert
        Assert.AreEqual(ExceptionMessages.PaymentLost, exception.Message);
    }
    
    [TestMethod]
    public async Task PaymentReceived_WhenPaymentExists_UpdatesDbAndPublishesEvent()
    {
        // Act
        var productClientMock = new Mock<ProductAPI.ProductAPIClient>();
        var paymentService = new ordersvc::OrderService.Services.PaymentService(_dbContext, productClientMock.Object, _bus);

        var paymentId = _paymentEntities.First(x => !x.Paid && x.ExpiresAt > DateTime.UtcNow).Id;

        var message = new PaymentSucceededEvent
        {
            PaymentId = paymentId
        };
        
        // Arrange
        await paymentService.PaymentReceivedAsync(message);

        // Assert
        _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
        Assert.AreEqual(true, _dbContext.Payments.First(x => x.Id == paymentId).Paid);
        _busMock.Verify(x => x.Publish(It.IsAny<OrderCompletedEvent>(), CancellationToken.None), Times.Once());
    }
}