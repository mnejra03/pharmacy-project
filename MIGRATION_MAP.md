# Pharmacy migration map

Source: `../NewPharmacy` and `../frontend`. Target: `Market.Backend` and `Market.Frontend/rs1-frontend-2025-26`.

The requested scope excludes catalog, cart, checkout, payment, and order flows. The template's demo catalog and sales-order modules will also be removed. JWT authentication and FluentValidation remain the template implementation.

| Source feature | Old backend endpoints / entities / DTOs | Old frontend screens / services | Target and disposition |
|---|---|---|---|
| Authentication and accounts | `AuthEndpoint/*`; `MyAppUserEndpoints/*`; `MyAppUser`, `MyAuthenticationToken`; `RegisterUserDTO`, `CreateMyAppUserDTO`, `UpdateMyAppUserDTO` | `auth/{login,register}`, auth/user services, admin `users` | Keep template JWT login/logout/refresh; add CQRS registration, profile/password updates, and paged admin user listing/edit/deactivation with FluentValidation. A new staff user registers first, then an administrator assigns the role. |
| Pharmacist profile | `PharmacistEndpoints/*`; shared `MyAppUser` role/profile data | pharmacist profile/dashboard and profile service | Profile/password use cases use CQRS and template JWT identity; profile image upload uses the backend local file store. |
| Chat | `ChatEndpoints/*`; `Chat`, `ChatCreateDTO`, `ChatGetDTO`; `ChatHub` | public chat and chat service | Authenticated HTTP history/send and SignalR push are migrated. Pharmacists can reply to customers; contacts are scoped to the user. |
| Notifications | `NotificationEndpoints/*`; `Notification`, `FullNotificationDTO` | pharmacist notifications and notification service | Migrate user-scoped list, read, unread, and delete. New customer chat messages notify pharmacists and are pushed over SignalR. Order-linked details are removed. |
| Recipes / prescription scans | `RecipeEndpoints/*`; `Recipe`; upload DTO nested in `RecipeEndpoints.cs` | pharmacist/customer recipes and recipe service | Migrate customer submission, pharmacist review UI/status updates, and protected scan download. Scan bytes remain in the database (10 MB maximum). |
| Advertisements and media | `AdvertisementEndpoints/*`; `Advertisement`; image updates; `AzureBlobService` | public advertisement, admin media, advertisement service | Migrate advertisement CRUD, public slideshow, and admin image upload. Uploads are stored under backend `wwwroot/uploads`; legacy Azure Blob credentials/provider are not copied. |
| Pharmacist stock / supplier orders | `OrderEndpoints/PostOrderMedicineEndpoint`, `GetMySupplyOrdersEndpoint`, `UpdateSupplyOrderStatusEndpoint`; `Supplier`, supply `Order` | pharmacist `stock`, `customer-order`, `my-orders`; `stock.service`, `order.service` | Exclude because these screens and flows depend on the removed medicine catalog/order domain. |
| Admin dashboard | `AdminEndpoints/GetDashboardEndpoint`, `DashboardStatsDto` | admin dashboard and service | Rebuild user, pharmacist, recipe, pending-recipe, and unread-notification counts. No sales, order, or stock metrics. |
| Catalog, category, brand, cart, checkout, payment, customer orders | `ProductEndpoints/*`, `CategoryEndpoints/*`, `BrandsEndpoint/*`, `CartEndpoints/*`, `OrderEndpoints/*`, `OrderDetailEndpoints/*`, `PaymentEndpoints/*`; `Product`, `Category`, `Brand`, `Cart`, `CartDetail`, `Order`, `OrderDetail`, `Sale`, `Discount`; product/cart/order/payment DTOs | public product/category/brand/search/details/cart/checkout/orders; admin products/orders; related product/category/brand/cart/order services | Remove from template and do not migrate, per explicit scope. |
| Favorites and product reviews | `WishListEndpoints/*`, `ReviewEndpoints/*`; `WishList`, `WishListDetail`, `Review`; `WishListItemUpsertDTO`, `ReviewDTO` | public `wish-list`, product review form; `wishlist.service`, `review.service` | Exclude because both are tied to the removed product catalog. |
| Shared images and UI behavior | product/brand/ad image upload endpoints; `FileHelper` | product image preview; advertisement slideshow; shared image assets | Migrate only generic/advertisement/recipe media. The old product zoom/download behavior is catalog-bound and excluded with catalog screens. |

## Target template baseline

- Backend layers: `Market.Domain`, `Market.Application`, `Market.Infrastructure`, `Market.API`, `Market.Shared`.
- Existing template demo entities/flows to remove: `ProductEntity`, `ProductCategoryEntity`, `OrderEntity`, `OrderItemEntity`, corresponding CQRS modules/controllers/configurations/seed data and EF migrations.
- Preserve `MarketUserEntity`, `RefreshTokenEntity`, JWT services, exception middleware, validation pipeline, and FluentValidation registration.
- Frontend stays NgModule-based and non-standalone, with lazy-loaded auth/admin/client/public modules. Remove demo catalog/order components and API services; new API services remain transport-only.

## Remaining technical constraints

- The source project has no mail/SMS token delivery implementation to migrate; outbound activation/reset invitations remain unimplemented.
- Drag-and-drop behavior is not present in the source feature screens.
- Local development uses EF InMemory and resets data on backend restart; non-development environments use SQL Server migrations. Local file storage requires persistent writable backend storage in production; Azure Blob credentials/provider were not migrated.
- Recipe scan bytes are stored in the database, with a 10 MB upload limit.
