# Investment Backend API

Una REST API desarrollada en .NET 8 siguiendo la arquitectura limpia (Clean Architecture) y principios de Domain-Driven Design (DDD), con integración a Amazon DynamoDB.

## 🏗️ Arquitectura

El proyecto sigue la arquitectura limpia con las siguientes capas:

```
src/
├── InvestmentBackend.Domain/          # Capa de Dominio
│   ├── Entities/                      # Entidades del dominio
│   ├── ValueObjects/                  # Objetos de valor
│   └── Repositories/                  # Interfaces de repositorios
├── InvestmentBackend.Application/     # Capa de Aplicación
│   └── Investments/
│       ├── Commands/                  # Comandos CQRS
│       └── Queries/                   # Consultas CQRS
├── InvestmentBackend.Infrastructure/  # Capa de Infraestructura
│   ├── Configuration/                 # Configuración de DI
│   └── Repositories/                  # Implementaciones de repositorios
└── InvestmentBackend.WebApi/          # Capa de Presentación
    └── Controllers/                   # Controladores de API
```

## 🚀 Características

- **Clean Architecture**: Separación clara de responsabilidades
- **Domain-Driven Design**: Modelado rico del dominio
- **CQRS**: Separación de comandos y consultas usando MediatR
- **DynamoDB**: Base de datos NoSQL para alta escalabilidad
- **Swagger/OpenAPI**: Documentación automática de la API
- **Validación**: Validaciones de dominio y reglas de negocio

## 📋 Endpoints

### Investments API

- `GET /api/investments` - Obtener todas las inversiones
- `GET /api/investments/{id}` - Obtener inversión por ID
- `POST /api/investments` - Crear nueva inversión
- `PUT /api/investments/{id}` - Actualizar inversión existente
- `DELETE /api/investments/{id}` - Eliminar inversión

## 🛠️ Tecnologías

- **.NET 8** - Framework principal
- **MediatR** - Implementación de patrón Mediator para CQRS
- **AWS SDK DynamoDB** - Cliente para Amazon DynamoDB
- **Swagger/OpenAPI** - Documentación de API
- **Entity Framework Core** - ORM (opcional para futuras extensiones)

## 🏃‍♂️ Ejecutar el proyecto

### Prerrequisitos

- .NET 8 SDK
- AWS CLI configurado (para producción) o DynamoDB Local (para desarrollo)

### Desarrollo

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd investment_backend
   ```

2. **Compilar la solución**
   ```bash
   dotnet build
   ```

3. **Ejecutar la aplicación**
   ```bash
   dotnet run --project src/InvestmentBackend.WebApi
   ```

4. **Acceder a Swagger UI**
   ```
   http://localhost:5005/swagger
   ```

### Configuración de DynamoDB

#### Desarrollo (DynamoDB Local)
```json
{
  "AWS": {
    "Region": "us-east-1",
    "DynamoDB": {
      "LocalMode": true,
      "LocalServiceUrl": "http://localhost:8000"
    }
  }
}
```

#### Producción
```json
{
  "AWS": {
    "Region": "us-east-1",
    "DynamoDB": {
      "LocalMode": false
    }
  }
}
```

## 📝 Ejemplos de uso

### Crear una inversión
```bash
curl -X POST "http://localhost:5005/api/investments" \\
     -H "Content-Type: application/json" \\
     -d '{
       "name": "Apple Stock Investment",
       "description": "Investment in Apple Inc. shares",
       "amount": 10000.50,
       "investmentType": "Stock",
       "expectedReturn": 12.5,
       "currency": "USD",
       "startDate": "2025-01-01T00:00:00Z",
       "riskLevel": "Medium"
     }'
```

### Obtener todas las inversiones
```bash
curl -X GET "http://localhost:5005/api/investments" \\
     -H "Accept: application/json"
```

## 🏗️ Estructura del Dominio

### Entidades
- **Investment**: Entidad principal que representa una inversión

### Value Objects
- **Money**: Representa valores monetarios con validaciones
- **InvestmentType**: Tipos de inversión (Stock, Bond, etc.)
- **RiskLevel**: Niveles de riesgo (Low, Medium, High, VeryHigh)

### Agregados
- **Investment Aggregate**: Gestiona el ciclo de vida de las inversiones

## 🧪 Testing

El proyecto incluye archivos HTTP para probar la API:
- `src/InvestmentBackend.WebApi/InvestmentBackend.WebApi.http`

## 📚 Próximas mejoras

- [ ] Implementar autenticación y autorización
- [ ] Agregar logging estructurado
- [ ] Implementar caché con Redis
- [ ] Agregar métricas y monitoreo
- [ ] Implementar eventos de dominio
- [ ] Agregar tests unitarios e integración
- [ ] Implementar paginación en consultas
- [ ] Agregar validación con FluentValidation

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.
