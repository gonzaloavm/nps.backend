# NPS Backend API

El backend para el sistema de NPS (Net Promoter Score). Es una API RESTful desarrollada en **.NET 10** enfocada en alto rendimiento, mantenibilidad y separación de conceptos.

## Tecnologías Principales

- **Framework:** .NET 10 (ASP.NET Core Web API)
- **Base de Datos:** SQL Server
- **Micro-ORM:** Dapper
- **Mediador (CQRS):** MediatR
- **Validación:** FluentValidation
- **Autenticación:** JWT (JSON Web Tokens)
- **Documentación API:** OpenAPI integrado con **Scalar**

## Arquitectura y Patrones Utilizados

El proyecto sigue lineamientos de **Clean Architecture**, estructurado en cuatro capas o proyectos principales:

1. **Domain:** Contiene las entidades del dominio de negocio e interfaces abstractas. No depende de ninguna otra capa.
2. **Application:** Contiene toda la lógica de negocio estructurada alrededor del patrón **CQRS (Command-Query Responsibility Segregation)** usando MediatR. Aquí viven los DTOs, validaciones, *Features* (Commands & Queries), y comportamientos del pipeline de MediatR (Pipeline Behaviors).
3. **Infrastructure:** Contiene la implementación concreta del acceso a datos, servicios externos y extensiones. Implementa el **patrón Repository**. Utiliza `DapperContext` para manejar la conexión ligera con la base de datos de manera óptima.
4. **WebAPI:** Es la capa de presentación (API Controllers). Gestiona las peticiones HTTP, middleware global para manejo de excepciones, y la configuración de inyección de dependencias.

### Patrones y Decisiones de Diseño Clave

- **Result Pattern:** En lugar de lanzar excepciones para flujos de control (como "usuario no encontrado" o "credenciales inválidas"), los *Handlers* devuelven un objeto `Result<T>` estructurado. Esto centraliza y encapsula si una operación fue exitosa o falló, entregando códigos de error de negocio tipificados, evitando los costosos `throw catch` en el servidor y mejorando la predictibilidad de la API.
- **CQRS con MediatR:** Se separan las operaciones de lectura (Queries) y escritura (Commands). Esto aumenta drásticamente la claridad de la lógica de negocio individual para cada "Feature".
- **Validation Pipeline Behavior:** Se captura la validación (usando `FluentValidation`) en el *Pipeline* de MediatR interrumpiendo tempranamente si las request entrantes no cumplen con las reglas de negocio base, previniendo invalidar lógica profunda.
- **Repository Pattern:** Abstracción completa del acceso a base de datos. A pesar de usar Dapper, el "Application Layer" desconoce las queries SQL gracias a que dialoga únicamente bajo las abstracciones (`IRepositoryBase`, u otros).
- **Global Exception Handler:** Middleware global que captura errores de la aplicación y estandariza las respuestas `500 Server Error`, lo que mejora la experiencia consumidora del cliente.
- **Docker Multi-stage Build:** Un Dockerfile `.slnx` estructurado que primero compila usando SDK de .NET 10 y luego mueve solo los dlls resultantes a una imagen puramente `aspnet:10.0` logrando contenedores de runtime mínimos.

## Despliegue Local

Para correr únicamente el API de forma local sin Docker:

```bash
cd nps.backend
dotnet restore
cd WebAPI
dotnet run
```
*Asegúrate de ajustar los `ConnectionStrings` dentro de `appsettings.json` o `appsettings.Development.json`.*
