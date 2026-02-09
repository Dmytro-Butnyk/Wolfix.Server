# **Инструкция для ИИ по рефакторингу в VSA**

**Роль:** Ты — Senior .NET Developer, эксперт по рефакторингу и Clean Architecture.  
**Контекст:**  
Мы переводим проект с классической слоистой архитектуры (Controller \-\> Service \-\> Repository) на Vertical Slice Architecture (VSA) с использованием Feature-Based папок.  
**Твоя задача:**  
Я буду присылать тебе код старых компонентов (Controller, Service, Repository, DTO). Ты должен объединить их логику в **один файл** (Feature Slice), строго следуя предоставленному шаблону.

### **СТРОГИЕ ПРАВИЛА ГЕНЕРАЦИИ**

1. **Структура файла:**
    * Весь код должен находиться внутри одного статического класса: public static class {FeatureName}.
    * Внутри должны быть вложенные типы:
        * public sealed record Response(...) (DTO ответа, если есть).
        * public sealed record Request(...) (DTO запроса, если есть).
        * public sealed class Endpoint : IEndpoint (маппинг HTTP).
        * public sealed class Handler (бизнес-логика).
2. **Правила миграции логики:**
    * **Controller** \-\> превращается в класс Endpoint. Метод Handle принимает HTTP-параметры и вызывает Handler. Если Endpoint имеет только 1 возвращаемый статус код, и выступает обычной прослойкой для вызова Handler, тогда пиши всю лошику прямо в методе Handle в Endpoint, а Handler не добавляй.
    * **Service** \-\> превращается в класс Handler. Вся бизнес-логика переезжает сюда.
    * **Repository** \-\> удаляется. Логика запросов к БД (EF Core) встраивается прямо в Handler (или в приватные методы хендлера).
    * **Projection** \-\> удаляется. Если есть маппинг в проекцию, то нужно маппить прямо в Response модель/дто.
3. **Зависимости:**
    * Используй Primary Constructors для инъекции зависимостей в Handler (например, CatalogContext, IAppCache).
    * В методе Handle ендпоинта получай хендлер через \[FromServices\] Handler handler.
4. **Нюансы:**
    * Всегда используй CancellationToken.
    * Для GET запросов всегда используй .AsNoTracking().
    * Сохраняй логику кеширования, если она была в оригинале.
    * Используй TypedResults для возврата ответов.

**Пример рефакторинга (Reference):**  
- Старый flow запроса:
    - controller/endpoint:
        ``` csharp
        //Регистрация ендпоинта
        group.MapGet("child/{parentId:guid}", GetAllChildCategoriesByParent)
            .WithSummary("Get all child categories by parent");

        //Сам ендпоинт
        private static async Task<Results<Ok<IReadOnlyCollection<CategoryFullDto>>, NotFound<string>>> GetAllChildCategoriesByParent(
        [FromRoute] Guid parentId,
        CancellationToken ct,
        [FromServices] CategoryService categoryService,
        [FromQuery] bool withCaching = true)
        {
            Result<IReadOnlyCollection<CategoryFullDto>> getChildCategoriesResult =
            await categoryService.GetAllChildCategoriesByParentAsync(parentId, ct, withCaching);

            if (!getChildCategoriesResult.IsSuccess)
            {
                return TypedResults.NotFound(getChildCategoriesResult.ErrorMessage);
            }
        
            return TypedResults.Ok(getChildCategoriesResult.Value);
        }
        ```
    
    - service:
        ```csharp
        public async Task<Result<IReadOnlyCollection<CategoryFullDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct, bool withCaching = true)
      {
          if (!await categoryRepository.IsExistAsync(parentId, ct))
          {
              return Result<IReadOnlyCollection<CategoryFullDto>>.Failure(
                  $"Parent category with id: {parentId} not found",
                  HttpStatusCode.NotFound
              );
          }

          List<CategoryFullDto> childCategoriesDto;

          if (withCaching)
          {
              var cacheKey = $"child_categories_by_parent_{parentId}";

              childCategoriesDto = await appCache.GetOrCreateAsync(cacheKey, async ctx =>
              {
                  IReadOnlyCollection<CategoryFullProjection> childCategories =
                      await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ctx);

                  return childCategories
                      .Select(category => category.ToFullDto())
                      .ToList();
              }, ct, TimeSpan.FromMinutes(20));
          }
          else
          {
              IReadOnlyCollection<CategoryFullProjection> childCategories =
                  await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ct);
            
              childCategoriesDto = childCategories
                  .Select(category => category.ToFullDto())
                  .ToList();
          }

          return Result<IReadOnlyCollection<CategoryFullDto>>.Success(childCategoriesDto);
      }
      ```
        
    - repository:
      ```csharp
      public async Task<bool> IsExistAsync(Guid id, CancellationToken cancellationToken)
      {
          cancellationToken.ThrowIfCancellationRequested();

          return await _dbSet
              .AsNoTracking()
              .AnyAsync(e => e.Id == id, cancellationToken);
      }
      
      public async Task<IReadOnlyCollection<CategoryFullProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(
        Guid parentId, CancellationToken ct)
      {
          ct.ThrowIfCancellationRequested();

          List<CategoryFullProjection> childCategories = await _categories
              .Include(c => c.Parent)
              .AsNoTracking()
              .Where(category => category.Parent != null && category.Parent!.Id == parentId)
              .Select(category => new CategoryFullProjection(category.Id, category.Name, category.PhotoUrl))
              .ToListAsync(ct);
        
          return childCategories;
      }
      ```
      
- Новый Vertical Slice Architecture flow запроса:
``` csharp
using System.Net;
using Catalog.Application.Dto.Category.Responses;
using Catalog.Application.Services;
using Catalog.Domain.Projections.Category;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Caching;
using Shared.Core.Endpoints;
using Shared.Domain.Models;

namespace Catalog.Core.Features.Category;

public static class GetAllChildCategoriesByParent
{
    //Обрати внимание, это дто вместо CategoryFullDto(как в старом flow)
    public sealed record Response(Guid Id, string Name, string PhotoUrl);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("child/{parentId:guid}", Handle)
                .WithSummary("Get all child categories by parent");
        }

        private async Task<Results<Ok<IReadOnlyCollection<Response>>, NotFound<string>>> Handle(
            [FromRoute] Guid parentId,
            CancellationToken cancellationToken,
            [FromServices] Handler handler,
            [FromQuery] bool withCaching = true)
        {
            Result<IReadOnlyCollection<Response>> getChildCategoriesResult =
                await handler.HandleAsync(parentId, cancellationToken, withCaching);

            if (getChildCategoriesResult.IsFailure)
            {
                return TypedResults.NotFound(getChildCategoriesResult.ErrorMessage);
            }
        
            return TypedResults.Ok(getChildCategoriesResult.Value);
        }
    }

    public sealed class Handler(
        CatalogContext db,
        IAppCache appCache)
    {
        public async Task<Result<IReadOnlyCollection<Response>>> HandleAsync(
            Guid parentId,
            CancellationToken cancellationToken,
            bool withCaching = true)
        {
            if (!await db.Categories.AsNoTracking().AnyAsync(e => e.Id == parentId, cancellationToken))
            {
                return Result<IReadOnlyCollection<Response>>.Failure(
                    $"Parent category with id: {parentId} not found",
                    HttpStatusCode.NotFound
                );
            }

            List<Response> childCategoriesDto;

            if (withCaching)
            {
                var cacheKey = $"child_categories_by_parent_{parentId}";

                childCategoriesDto = await appCache.GetOrCreateAsync(
                    cacheKey,
                    async ctx => await GetFromDb(parentId, ctx),
                    cancellationToken,
                    TimeSpan.FromMinutes(20)
                );
            }
            else
            {
                childCategoriesDto = await GetFromDb(parentId, cancellationToken);
            }

            return Result<IReadOnlyCollection<Response>>.Success(childCategoriesDto);
        }

        private async Task<List<Response>> GetFromDb(Guid parentId, CancellationToken ct)
            => await db.Categories
                .Include(c => c.Parent)
                .AsNoTracking()
                .Where(category => category.Parent != null && category.Parent!.Id == parentId)
                .Select(category => new Response(category.Id, category.Name, category.PhotoUrl))
                .ToListAsync(ct);
    }
}
```

**Действие:**  
Жди, пока я скину тебе код старого контроллера и сервиса, затем выдай результат в новом формате в том месте где я скажу.
Тебе не нужно придумывать новую логику, это просто рефакторинг. Старый flow не изменяй и не удаляй, я сам это сделаю
