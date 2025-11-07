# shop-microservices

[![Build & test .NET Aspire](https://github.com/AdisonCavani/shop-microservices/actions/workflows/build.yaml/badge.svg)](https://github.com/AdisonCavani/shop-microservices/actions/workflows/build.yaml)

Shop microservices project

## Tech stack

- Framework - ASP.NET
- Database - PostgreSQL
- ORM - Entity Framework
- Email handling - MailKit (smtp4dev), liquid templates (fluid)
- Message bus - RabbitMQ via MassTransit
- Auth - JWT Bearer
- Validation - FluentValidation
- API - Minimal API

### smtp4dev UI

```
http://localhost:5000
```

### Stripe webhook

Forward request via [`stripe-cli`](https://docs.stripe.com/stripe-cli)

```
stripe listen --latest --forward-to https://localhost:7040/api/payment/webhook
```
