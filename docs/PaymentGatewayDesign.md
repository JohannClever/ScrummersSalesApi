# Diseño de Pasarela de Pagos para ScrummersSalesApi

## Índice
1. [Introducción](#introducción)
2. [Ejemplos de Referencia en GitHub](#ejemplos-de-referencia-en-github)
3. [Arquitectura Propuesta](#arquitectura-propuesta)
4. [Componentes del Sistema](#componentes-del-sistema)
5. [Estructura de Código](#estructura-de-código)
6. [Flujo de Procesamiento de Pagos](#flujo-de-procesamiento-de-pagos)
7. [Integración con el Sistema Actual](#integración-con-el-sistema-actual)
8. [Consideraciones de Seguridad](#consideraciones-de-seguridad)
9. [Patrones de Diseño Recomendados](#patrones-de-diseño-recomendados)

---

## Introducción

Este documento describe el diseño de una **pasarela de pagos simple y fácil de implementar** para el sistema ScrummersSalesApi. La solución está basada en las mejores prácticas encontradas en repositorios públicos de GitHub y adaptada a la arquitectura DDD existente del proyecto.

### Objetivos del Diseño
- ✅ Integración sencilla con el servicio de Orders existente
- ✅ Soporte para múltiples proveedores de pago (Stripe, PayPal, etc.)
- ✅ Diseño extensible y mantenible
- ✅ Cumplimiento de estándares de seguridad PCI-DSS
- ✅ Seguir principios SOLID y Clean Architecture

---

## Ejemplos de Referencia en GitHub

### Repositorios Destacados

1. **Stripe.net** (Oficial)
   - URL: `https://github.com/stripe/stripe-dotnet`
   - Características: SDK oficial de Stripe para .NET, muy completo y bien documentado
   - Uso: Como referencia para implementar el adapter de Stripe

2. **PayPal .NET SDK**
   - URL: `https://github.com/paypal/PayPal-NET-SDK`
   - Características: SDK oficial de PayPal para integración de pagos
   - Uso: Referencia para múltiples métodos de pago

3. **NopCommerce Payment Plugins**
   - URL: `https://github.com/nopSolutions/nopCommerce`
   - Características: E-commerce completo con múltiples plugins de pago
   - Uso: Arquitectura de plugins de pago extensible

4. **SimplCommerce**
   - URL: `https://github.com/simplcommerce/SimplCommerce`
   - Características: Sistema modular con soporte para múltiples pasarelas
   - Uso: Patrón Strategy para proveedores de pago

5. **Payment Gateway Sample**
   - URL: `https://github.com/dotnet-architecture/eShopOnContainers`
   - Características: Arquitectura de microservicios con integración de pagos
   - Uso: Referencia para microservicios de pagos

---

## Arquitectura Propuesta

### Diagrama de Alto Nivel

```
┌─────────────────────────────────────────────────────────────────┐
│                         Client Application                       │
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Orders API (Existing)                       │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              OrdersController                             │  │
│  └────────────────────────┬─────────────────────────────────┘  │
│                           │                                      │
│                           ▼                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │         CreateOrderCommandHandler (Modified)             │  │
│  └────────────────────────┬─────────────────────────────────┘  │
└───────────────────────────┼──────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                    NEW: Payments Service                         │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              IPaymentService (Port)                       │  │
│  └────────────────────────┬─────────────────────────────────┘  │
│                           │                                      │
│         ┌─────────────────┼─────────────────┐                  │
│         ▼                 ▼                 ▼                    │
│  ┌──────────┐      ┌──────────┐     ┌──────────┐              │
│  │  Stripe  │      │  PayPal  │     │   Cash   │              │
│  │  Adapter │      │  Adapter │     │  Adapter │              │
│  └──────────┘      └──────────┘     └──────────┘              │
└─────────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                  External Payment Providers                      │
│              (Stripe API, PayPal API, etc.)                     │
└─────────────────────────────────────────────────────────────────┘
```

### Capas de la Arquitectura

**1. Domain Layer (Nuevo módulo de Payments)**
   - Entidades: `Payment`, `PaymentTransaction`, `Refund`
   - Value Objects: `Money`, `PaymentMethod`, `PaymentStatus`
   - Interfaces (Ports): `IPaymentService`, `IPaymentGateway`
   - Commands: `ProcessPaymentCommand`, `RefundPaymentCommand`
   - Queries: `GetPaymentByIdQuery`, `GetPaymentsByOrderQuery`

**2. Infrastructure Layer**
   - Adapters: `StripePaymentGateway`, `PayPalPaymentGateway`, `CashPaymentGateway`
   - Repository: `PaymentRepository`
   - Configuration: `PaymentDbContext`, Entity Configurations

**3. Application Layer (Integration)**
   - Modificación de `CreateOrderCommandHandler` para incluir procesamiento de pago
   - Nuevos handlers para comandos de pago

---

## Componentes del Sistema

### 1. Entidades del Dominio

#### Payment (Entidad Principal)
```csharp
public class Payment : EntityBase
{
    public string OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string TransactionId { get; private set; }
    public string ExternalReference { get; private set; }
    public DateTime ProcessedAt { get; private set; }
    public string ErrorMessage { get; private set; }
    
    // Métodos de negocio
    public void MarkAsCompleted(string transactionId)
    public void MarkAsFailed(string errorMessage)
    public void MarkAsRefunded()
}
```

#### PaymentTransaction (Registro de Transacciones)
```csharp
public class PaymentTransaction : EntityBase
{
    public string PaymentId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string GatewayResponse { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
```

### 2. Enumeraciones

```csharp
public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    Cancelled = 5
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    PayPal = 2,
    BankTransfer = 3,
    Cash = 4
}

public enum TransactionType
{
    Charge = 0,
    Refund = 1,
    Authorization = 2,
    Capture = 3
}
```

### 3. Value Objects

```csharp
public record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) => new Money(0, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return new Money(Amount + other.Amount, Currency);
    }
}
```

### 4. DTOs (Data Transfer Objects)

```csharp
// Request
public class ProcessPaymentRequest
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentDetails PaymentDetails { get; set; }
}

public class PaymentDetails
{
    // Para tarjeta de crédito
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string CVV { get; set; }
    
    // Para PayPal
    public string PayPalEmail { get; set; }
    
    // Token de pago (más seguro)
    public string PaymentToken { get; set; }
}

// Response
public class PaymentResponse
{
    public bool Success { get; set; }
    public string PaymentId { get; set; }
    public string TransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public string Message { get; set; }
    public DateTime ProcessedAt { get; set; }
}
```

---

## Estructura de Código

### Estructura de Directorios Propuesta

```
Backend/
└── ScrummersSalesApi.Backend.Payments/
    ├── ScrummersSalesApi.Backend.Payments.Domain/
    │   ├── Commands/
    │   │   ├── Command/
    │   │   │   ├── ProcessPaymentCommand.cs
    │   │   │   ├── RefundPaymentCommand.cs
    │   │   │   └── CancelPaymentCommand.cs
    │   │   └── Handler/
    │   │       ├── ProcessPaymentCommandHandler.cs
    │   │       ├── RefundPaymentCommandHandler.cs
    │   │       └── CancelPaymentCommandHandler.cs
    │   ├── Queries/
    │   │   ├── Query/
    │   │   │   ├── GetPaymentByIdQuery.cs
    │   │   │   └── GetPaymentsByOrderQuery.cs
    │   │   └── Handler/
    │   │       ├── GetPaymentByIdQueryHandler.cs
    │   │       └── GetPaymentsByOrderQueryHandler.cs
    │   ├── Entities/
    │   │   ├── Payment.cs
    │   │   └── PaymentTransaction.cs
    │   ├── ValueObjects/
    │   │   └── Money.cs
    │   ├── Enums/
    │   │   ├── PaymentStatus.cs
    │   │   ├── PaymentMethod.cs
    │   │   └── TransactionType.cs
    │   ├── Dto/
    │   │   ├── ProcessPaymentRequest.cs
    │   │   ├── PaymentResponse.cs
    │   │   └── PaymentDto.cs
    │   ├── Ports/
    │   │   ├── IPaymentService.cs
    │   │   ├── IPaymentGateway.cs
    │   │   └── IPaymentRepository.cs
    │   └── Services/
    │       └── PaymentService.cs
    │
    ├── ScrummersSalesApi.Backend.Payments.Infrastructure/
    │   ├── Adapters/
    │   │   ├── PaymentGateway/
    │   │   │   ├── StripePaymentGateway.cs
    │   │   │   ├── PayPalPaymentGateway.cs
    │   │   │   └── CashPaymentGateway.cs
    │   │   └── Repository/
    │   │       └── PaymentRepository.cs
    │   ├── DataAccess/
    │   │   ├── PaymentDbContext.cs
    │   │   └── Configuration/
    │   │       ├── PaymentConfiguration.cs
    │   │       └── PaymentTransactionConfiguration.cs
    │   └── Extensions/
    │       └── ServiceCollectionExtensions.cs
    │
    └── ScrummersSalesApi.Backend.Payments.Api/
        ├── Controllers/
        │   └── PaymentsController.cs
        ├── Program.cs
        └── appsettings.json
```

### Interfaces Principales (Ports)

#### IPaymentService
```csharp
namespace ScrummersSalesApi.Backend.Payments.Domain.Ports
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(
            ProcessPaymentRequest request, 
            CancellationToken cancellationToken = default);
            
        Task<PaymentResponse> RefundPaymentAsync(
            string paymentId, 
            decimal? amount = null,
            CancellationToken cancellationToken = default);
            
        Task<PaymentResponse> CancelPaymentAsync(
            string paymentId, 
            CancellationToken cancellationToken = default);
            
        Task<Payment> GetPaymentByIdAsync(
            string paymentId, 
            CancellationToken cancellationToken = default);
            
        Task<IEnumerable<Payment>> GetPaymentsByOrderAsync(
            string orderId, 
            CancellationToken cancellationToken = default);
    }
}
```

#### IPaymentGateway
```csharp
namespace ScrummersSalesApi.Backend.Payments.Domain.Ports
{
    public interface IPaymentGateway
    {
        PaymentMethod SupportedMethod { get; }
        
        Task<GatewayResponse> ChargeAsync(
            ChargeRequest request, 
            CancellationToken cancellationToken = default);
            
        Task<GatewayResponse> RefundAsync(
            RefundRequest request, 
            CancellationToken cancellationToken = default);
            
        Task<GatewayResponse> CancelAsync(
            string transactionId, 
            CancellationToken cancellationToken = default);
            
        Task<GatewayResponse> GetTransactionStatusAsync(
            string transactionId, 
            CancellationToken cancellationToken = default);
    }
}
```

### Implementación de Servicio

```csharp
namespace ScrummersSalesApi.Backend.Payments.Domain.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEnumerable<IPaymentGateway> _paymentGateways;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IEnumerable<IPaymentGateway> paymentGateways,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _paymentGateways = paymentGateways;
            _logger = logger;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(
            ProcessPaymentRequest request, 
            CancellationToken cancellationToken = default)
        {
            // 1. Validar request
            ValidatePaymentRequest(request);
            
            // 2. Crear entidad Payment
            var payment = CreatePaymentEntity(request);
            
            // 3. Guardar en estado Pending
            await _paymentRepository.AddAsync(payment);
            
            try
            {
                // 4. Seleccionar gateway apropiado
                var gateway = SelectGateway(request.Method);
                
                // 5. Procesar pago con el gateway
                payment.MarkAsProcessing();
                await _paymentRepository.UpdateAsync(payment);
                
                var chargeRequest = MapToChargeRequest(request);
                var gatewayResponse = await gateway.ChargeAsync(
                    chargeRequest, 
                    cancellationToken);
                
                // 6. Actualizar estado según respuesta
                if (gatewayResponse.Success)
                {
                    payment.MarkAsCompleted(gatewayResponse.TransactionId);
                }
                else
                {
                    payment.MarkAsFailed(gatewayResponse.ErrorMessage);
                }
                
                await _paymentRepository.UpdateAsync(payment);
                
                // 7. Retornar respuesta
                return MapToPaymentResponse(payment, gatewayResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment {PaymentId}", payment.Id);
                payment.MarkAsFailed(ex.Message);
                await _paymentRepository.UpdateAsync(payment);
                throw;
            }
        }
        
        private IPaymentGateway SelectGateway(PaymentMethod method)
        {
            var gateway = _paymentGateways
                .FirstOrDefault(g => g.SupportedMethod == method);
                
            if (gateway == null)
            {
                throw new InvalidOperationException(
                    $"No payment gateway found for method {method}");
            }
            
            return gateway;
        }
        
        // Otros métodos...
    }
}
```

### Implementación de Gateway (Ejemplo: Stripe)

```csharp
namespace ScrummersSalesApi.Backend.Payments.Infrastructure.Adapters.PaymentGateway
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeClient _stripeClient;
        private readonly ILogger<StripePaymentGateway> _logger;

        public PaymentMethod SupportedMethod => PaymentMethod.CreditCard;

        public StripePaymentGateway(
            IOptions<StripeSettings> settings,
            ILogger<StripePaymentGateway> logger)
        {
            _stripeClient = new StripeClient(settings.Value.SecretKey);
            _logger = logger;
        }

        public async Task<GatewayResponse> ChargeAsync(
            ChargeRequest request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Stripe usa centavos
                    Currency = request.Currency.ToLower(),
                    PaymentMethod = request.PaymentToken,
                    Confirm = true,
                    Description = $"Payment for Order {request.OrderId}"
                };

                var service = new PaymentIntentService(_stripeClient);
                var intent = await service.CreateAsync(options, cancellationToken);

                return new GatewayResponse
                {
                    Success = intent.Status == "succeeded",
                    TransactionId = intent.Id,
                    Status = MapStripeStatus(intent.Status),
                    RawResponse = intent.StripeResponse.Content,
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe payment failed");
                return new GatewayResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ErrorCode = ex.StripeError?.Code
                };
            }
        }

        public async Task<GatewayResponse> RefundAsync(
            RefundRequest request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = request.TransactionId,
                    Amount = request.Amount.HasValue 
                        ? (long)(request.Amount.Value * 100) 
                        : null // null = refund completo
                };

                var service = new RefundService(_stripeClient);
                var refund = await service.CreateAsync(options, cancellationToken);

                return new GatewayResponse
                {
                    Success = refund.Status == "succeeded",
                    TransactionId = refund.Id,
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe refund failed");
                return new GatewayResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        
        // Otros métodos...
    }
}
```

### Implementación de Gateway Simple (Ejemplo: Cash)

```csharp
namespace ScrummersSalesApi.Backend.Payments.Infrastructure.Adapters.PaymentGateway
{
    public class CashPaymentGateway : IPaymentGateway
    {
        public PaymentMethod SupportedMethod => PaymentMethod.Cash;

        public Task<GatewayResponse> ChargeAsync(
            ChargeRequest request, 
            CancellationToken cancellationToken = default)
        {
            // Para pagos en efectivo, simplemente registramos la intención
            // La confirmación real se hará cuando se reciba el efectivo
            var response = new GatewayResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                Status = "pending_confirmation",
                ProcessedAt = DateTime.UtcNow,
                Message = "Cash payment registered, pending confirmation"
            };

            return Task.FromResult(response);
        }

        public Task<GatewayResponse> RefundAsync(
            RefundRequest request, 
            CancellationToken cancellationToken = default)
        {
            // Para efectivo, registrar el reembolso para seguimiento
            var response = new GatewayResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                ProcessedAt = DateTime.UtcNow,
                Message = "Cash refund registered"
            };

            return Task.FromResult(response);
        }

        public Task<GatewayResponse> CancelAsync(
            string transactionId, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new GatewayResponse
            {
                Success = true,
                ProcessedAt = DateTime.UtcNow
            });
        }

        public Task<GatewayResponse> GetTransactionStatusAsync(
            string transactionId, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new GatewayResponse
            {
                Success = true,
                Status = "completed",
                TransactionId = transactionId
            });
        }
    }
}
```

---

## Flujo de Procesamiento de Pagos

### Secuencia de Creación de Orden con Pago

```
Cliente -> Orders API: POST /api/orders (con datos de pago)
    |
    v
OrdersController -> CreateOrderCommand: Envía comando
    |
    v
CreateOrderCommandHandler:
    1. Validar orden
    2. Llamar OrderService.CreateOrderAsync()
    3. *** NUEVO: Llamar PaymentService.ProcessPaymentAsync() ***
    4. Si pago exitoso:
        - Marcar orden como "Paid"
        - Retornar respuesta con OrderId y PaymentId
    5. Si pago falla:
        - Marcar orden como "PaymentFailed"
        - Retornar error con detalles
    |
    v
PaymentService.ProcessPaymentAsync():
    1. Crear registro de Payment (estado: Pending)
    2. Seleccionar IPaymentGateway apropiado según PaymentMethod
    3. Llamar gateway.ChargeAsync()
    4. Actualizar Payment según resultado
    5. Retornar PaymentResponse
    |
    v
IPaymentGateway (ej: StripePaymentGateway):
    1. Preparar request al API externo
    2. Llamar Stripe API
    3. Mapear respuesta a GatewayResponse
    4. Retornar resultado
    |
    v
Cliente <- Orders API: Respuesta con OrderId, PaymentId, Status
```

### Diagrama de Estados del Payment

```
┌─────────┐
│ Pending │ (Inicial)
└────┬────┘
     │
     v
┌────────────┐
│ Processing │ (Durante llamada a gateway)
└─────┬──────┘
      │
      ├──────────┐
      v          v
┌──────────┐  ┌────────┐
│Completed │  │ Failed │
└────┬─────┘  └────────┘
     │
     v
┌──────────┐
│ Refunded │ (Después de refund)
└──────────┘
```

---

## Integración con el Sistema Actual

### Modificaciones Necesarias en el Servicio de Orders

#### 1. Actualizar OrderStatus
```csharp
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    // NUEVOS:
    PaymentPending = 2,
    PaymentFailed = 3,
    Paid = 4,
    // Existentes:
    Shipped = 5,
    Delivered = 6,
    Cancelled = 7
}
```

#### 2. Actualizar Order Entity
```csharp
public class Order : EntityBase
{
    // Propiedades existentes...
    
    // NUEVAS:
    public string PaymentId { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    
    public void AssociatePayment(string paymentId, PaymentStatus status)
    {
        PaymentId = paymentId;
        PaymentStatus = status;
        
        if (status == PaymentStatus.Completed)
        {
            Status = OrderStatus.Paid;
        }
        else if (status == PaymentStatus.Failed)
        {
            Status = OrderStatus.PaymentFailed;
        }
    }
}
```

#### 3. Actualizar CreateOrderCommand
```csharp
public class CreateOrderCommand : IRequest<CreateOrderCommandResponse>
{
    // Propiedades existentes...
    
    // NUEVAS:
    public ProcessPaymentRequest? PaymentRequest { get; set; }
    public bool ProcessPaymentImmediately { get; set; } = true;
}
```

#### 4. Modificar CreateOrderCommandHandler
```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResponse>
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService; // NUEVO
    
    public async Task<CreateOrderCommandResponse> Handle(
        CreateOrderCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Crear orden (lógica existente)
        var orderId = await _orderService.CreateOrderAsync(...);
        
        // 2. NUEVO: Procesar pago si se solicita
        PaymentResponse? paymentResponse = null;
        if (request.ProcessPaymentImmediately && request.PaymentRequest != null)
        {
            request.PaymentRequest.OrderId = orderId;
            paymentResponse = await _paymentService.ProcessPaymentAsync(
                request.PaymentRequest, 
                cancellationToken);
                
            // 3. Actualizar orden con información de pago
            if (paymentResponse.Success)
            {
                await _orderService.UpdatePaymentStatusAsync(
                    orderId, 
                    paymentResponse.PaymentId,
                    PaymentStatus.Completed);
            }
            else
            {
                await _orderService.UpdatePaymentStatusAsync(
                    orderId, 
                    null,
                    PaymentStatus.Failed);
            }
        }
        
        // 4. Retornar respuesta
        return new CreateOrderCommandResponse
        {
            OrderId = orderId,
            PaymentId = paymentResponse?.PaymentId,
            PaymentStatus = paymentResponse?.Status,
            Success = paymentResponse?.Success ?? true
        };
    }
}
```

### Configuración en appsettings.json

```json
{
  "PaymentGateways": {
    "Stripe": {
      "Enabled": true,
      "SecretKey": "sk_test_...",
      "PublishableKey": "pk_test_...",
      "WebhookSecret": "whsec_..."
    },
    "PayPal": {
      "Enabled": true,
      "ClientId": "...",
      "ClientSecret": "...",
      "Mode": "sandbox"
    },
    "Cash": {
      "Enabled": true
    }
  },
  "PaymentSettings": {
    "DefaultCurrency": "USD",
    "SupportedCurrencies": ["USD", "EUR", "MXN"],
    "DefaultTimeout": 30,
    "RetryAttempts": 3
  }
}
```

### Registro de Dependencias (DI)

```csharp
// En Infrastructure/Extensions/ServiceCollectionExtensions.cs
public static class PaymentServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuración
        services.Configure<StripeSettings>(
            configuration.GetSection("PaymentGateways:Stripe"));
        services.Configure<PayPalSettings>(
            configuration.GetSection("PaymentGateways:PayPal"));

        // Servicios del dominio
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Gateways (registrar todos)
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();
        services.AddScoped<IPaymentGateway, PayPalPaymentGateway>();
        services.AddScoped<IPaymentGateway, CashPaymentGateway>();

        // DbContext
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("database")));

        return services;
    }
}

// En Program.cs
builder.Services.AddPaymentServices(builder.Configuration);
```

---

## Consideraciones de Seguridad

### 1. PCI-DSS Compliance

**⚠️ NUNCA almacenar datos sensibles de tarjetas:**
- ❌ Números de tarjeta completos
- ❌ CVV/CVC
- ❌ Datos de autenticación

**✅ En su lugar:**
- ✅ Usar tokens de pago (Stripe Tokens, PayPal Tokens)
- ✅ Los datos sensibles van directamente del cliente al proveedor
- ✅ Nuestro servidor solo maneja tokens seguros

### 2. Implementación con Tokens

#### Frontend (JavaScript)
```javascript
// Ejemplo con Stripe.js (cliente)
const stripe = Stripe('pk_test_...');

async function handlePayment(orderData) {
    // 1. Crear PaymentMethod en el cliente
    const {paymentMethod, error} = await stripe.createPaymentMethod({
        type: 'card',
        card: cardElement, // Elemento de Stripe, nunca tocar datos raw
        billing_details: {
            name: customerName,
            email: customerEmail
        }
    });

    if (error) {
        console.error(error);
        return;
    }

    // 2. Enviar solo el token al backend
    const response = await fetch('/api/orders', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({
            ...orderData,
            paymentRequest: {
                method: 'CreditCard',
                paymentToken: paymentMethod.id, // Solo el token
                amount: totalAmount,
                currency: 'USD'
            }
        })
    });
}
```

### 3. Validación y Sanitización

```csharp
public class PaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .Length(1, 50);
            
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .LessThan(1000000);
            
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(BeValidCurrency)
            .WithMessage("Invalid currency code");
            
        RuleFor(x => x.PaymentDetails.PaymentToken)
            .NotEmpty()
            .When(x => x.Method == PaymentMethod.CreditCard)
            .WithMessage("Payment token is required for card payments");
            
        // NUNCA permitir números de tarjeta raw
        RuleFor(x => x.PaymentDetails.CardNumber)
            .Must(x => string.IsNullOrEmpty(x))
            .WithMessage("Card numbers should not be sent to the server");
    }
    
    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR", "MXN", "GBP" };
        return validCurrencies.Contains(currency);
    }
}
```

### 4. Logging Seguro

```csharp
public class SecurePaymentLogger
{
    public void LogPaymentAttempt(ProcessPaymentRequest request)
    {
        // ❌ NO HACER ESTO:
        // _logger.LogInformation("Payment: {@Request}", request);
        
        // ✅ HACER ESTO:
        _logger.LogInformation(
            "Payment attempt for Order {OrderId}, Amount {Amount} {Currency}, Method {Method}",
            request.OrderId,
            request.Amount,
            request.Currency,
            request.Method);
    }
    
    public void LogPaymentResult(PaymentResponse response)
    {
        _logger.LogInformation(
            "Payment {PaymentId} result: {Success}, Status: {Status}",
            MaskSensitiveData(response.PaymentId),
            response.Success,
            response.Status);
    }
    
    private string MaskSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data) || data.Length < 8)
            return "****";
            
        return $"{data.Substring(0, 4)}****{data.Substring(data.Length - 4)}";
    }
}
```

### 5. Manejo de Webhooks

Los proveedores de pago envían webhooks para notificar eventos asincrónicos:

```csharp
[ApiController]
[Route("api/webhooks/payments")]
public class PaymentWebhooksController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    [HttpPost("stripe")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        try
        {
            // Verificar firma del webhook
            var webhookSecret = _configuration["PaymentGateways:Stripe:WebhookSecret"];
            var stripeSignature = Request.Headers["Stripe-Signature"];
            
            var stripeEvent = EventUtility.ConstructEvent(
                json, 
                stripeSignature, 
                webhookSecret);

            // Procesar evento
            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    await HandlePaymentSucceeded(stripeEvent);
                    break;
                    
                case Events.PaymentIntentPaymentFailed:
                    await HandlePaymentFailed(stripeEvent);
                    break;
                    
                case Events.ChargeRefunded:
                    await HandleRefund(stripeEvent);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

---

## Patrones de Diseño Recomendados

### 1. Strategy Pattern (Gateways)

Permite cambiar dinámicamente entre diferentes proveedores de pago:

```csharp
public interface IPaymentGateway
{
    PaymentMethod SupportedMethod { get; }
    Task<GatewayResponse> ChargeAsync(ChargeRequest request);
}

// El servicio selecciona la estrategia correcta
public class PaymentService
{
    private IPaymentGateway SelectGateway(PaymentMethod method)
    {
        return _gateways.FirstOrDefault(g => g.SupportedMethod == method);
    }
}
```

### 2. Repository Pattern

Abstrae el acceso a datos:

```csharp
public interface IPaymentRepository
{
    Task<Payment> GetByIdAsync(string id);
    Task<IEnumerable<Payment>> GetByOrderIdAsync(string orderId);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}
```

### 3. Unit of Work Pattern

Gestiona transacciones:

```csharp
public class PaymentUnitOfWork : IUnitOfWork
{
    private readonly PaymentDbContext _context;
    
    public IPaymentRepository Payments { get; }
    public IPaymentTransactionRepository Transactions { get; }
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

### 4. Factory Pattern

Para crear objetos de dominio:

```csharp
public class PaymentFactory
{
    public static Payment CreatePayment(ProcessPaymentRequest request)
    {
        return new Payment
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = request.OrderId,
            Amount = request.Amount,
            Currency = request.Currency,
            Method = request.Method,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

### 5. Retry Pattern con Polly

Para reintentos en llamadas a APIs externas:

```csharp
public class ResilientPaymentGateway : IPaymentGateway
{
    private readonly IPaymentGateway _innerGateway;
    private readonly IAsyncPolicy<GatewayResponse> _retryPolicy;

    public ResilientPaymentGateway(IPaymentGateway innerGateway)
    {
        _innerGateway = innerGateway;
        
        _retryPolicy = Policy<GatewayResponse>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Log retry attempt
                });
    }

    public async Task<GatewayResponse> ChargeAsync(ChargeRequest request)
    {
        return await _retryPolicy.ExecuteAsync(() => 
            _innerGateway.ChargeAsync(request));
    }
}
```

### 6. Circuit Breaker Pattern

Para prevenir llamadas a servicios externos con fallas:

```csharp
var circuitBreakerPolicy = Policy<GatewayResponse>
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromMinutes(1),
        onBreak: (result, duration) =>
        {
            // Log: Circuit breaker opened
        },
        onReset: () =>
        {
            // Log: Circuit breaker reset
        });
```

---

## Resumen de Implementación

### Pasos para Implementar

1. **Crear nuevo microservicio de Payments**
   - Estructura de carpetas
   - Entidades del dominio
   - Interfaces (Ports)

2. **Implementar Adapters para cada Gateway**
   - StripePaymentGateway
   - PayPalPaymentGateway
   - CashPaymentGateway

3. **Crear PaymentService con lógica de negocio**
   - Validaciones
   - Orquestación de gateways
   - Manejo de errores

4. **Configurar persistencia**
   - Entity Framework DbContext
   - Configuraciones de entidades
   - Migrations

5. **Integrar con Orders Service**
   - Modificar CreateOrderCommandHandler
   - Actualizar Order entity
   - Ajustar DTOs

6. **Implementar API Controller**
   - Endpoints REST
   - Validación de entrada
   - Manejo de errores

7. **Configurar seguridad**
   - Tokens en lugar de datos raw
   - Validación de webhooks
   - Logging seguro

8. **Testing**
   - Unit tests para servicios
   - Integration tests con mocks
   - Tests de seguridad

### Paquetes NuGet Necesarios

```xml
<!-- Para Stripe -->
<PackageReference Include="Stripe.net" Version="43.0.0" />

<!-- Para PayPal -->
<PackageReference Include="PayPal" Version="1.9.1" />

<!-- Para Resiliencia -->
<PackageReference Include="Polly" Version="8.0.0" />
<PackageReference Include="Polly.Extensions.Http" Version="3.0.0" />

<!-- Para Validación -->
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />

<!-- Para Logging -->
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />

<!-- Para Testing -->
<PackageReference Include="Moq" Version="4.20.0" />
<PackageReference Include="xunit" Version="2.6.0" />
```

---

## Conclusión

Este diseño proporciona una **arquitectura limpia, extensible y segura** para procesar pagos en el sistema ScrummersSalesApi. Los puntos clave son:

✅ **Separación de responsabilidades**: Cada componente tiene una función clara  
✅ **Extensibilidad**: Fácil agregar nuevos proveedores de pago  
✅ **Seguridad**: Cumple con estándares PCI-DSS usando tokens  
✅ **Resiliencia**: Manejo robusto de errores y reintentos  
✅ **Integración suave**: Mínimos cambios en el sistema existente  

### Próximos Pasos Recomendados

1. Revisar y aprobar este diseño
2. Crear historias de usuario para implementación
3. Empezar con un gateway simple (ej: Cash)
4. Agregar Stripe como primer gateway completo
5. Implementar testing exhaustivo
6. Agregar monitoring y alertas
7. Documentar para el equipo

### Referencias Adicionales

- Documentación oficial de Stripe: https://stripe.com/docs/api
- Documentación oficial de PayPal: https://developer.paypal.com/docs/api/
- PCI-DSS Compliance Guide: https://www.pcisecuritystandards.org/
- Polly Documentation: https://github.com/App-vNext/Polly
- Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html

---

**Autor**: Diseño basado en mejores prácticas de la industria y ejemplos de GitHub  
**Fecha**: 2025  
**Versión**: 1.0
