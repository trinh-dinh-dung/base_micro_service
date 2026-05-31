# React App — Chiều ngang vs theo Page

## Hai trục tách bạch

| Trục | Thư mục | Ý nghĩa |
|------|---------|---------|
| **Chiều ngang (domain)** | `features/` | Nghiệp vụ dùng **≥ 2 page** hoặc toàn app: `auth`, `user` |
| **Theo page (dọc)** | `pages/<tên-page>/` | UI + state **chỉ page đó**: `home/model`, `home/sections` |

```
src/
├── app/                         # store, providers, routes
├── features/                    # DOMAIN — không gắn 1 page
│   ├── auth/                    # token SSO → axios
│   └── user/                    # slice, thunk, selectors (Home + /user)
├── pages/
│   ├── home/
│   │   ├── HomePage.tsx         # chỉ compose
│   │   ├── components/          # UI chỉ Home (header, hero, arch…)
│   │   ├── sections/            # block lớn trên Home
│   │   │   ├── UserInfoSection/ # UI → gọi features/user
│   │   │   └── ServiceHealthSection/
│   │   └── model/               # Redux chỉ Home (ping API)
│   ├── user/
│   │   └── UserPage.tsx         # UI → đọc features/user
│   └── callback/
└── shared/                      # api, config, ui, styles
```

## Khi nào đặt ở đâu?

| Tình huống | Đặt tại |
|------------|---------|
| User profile API, dùng Home + `/user` | `features/user/` |
| Ping health chỉ trên Home | `pages/home/model/` + `pages/home/sections/` |
| Header Home | `pages/home/components/` |
| SSO, axios token | `features/auth/` |
| Button, Loading dùng mọi nơi | `shared/ui/` |

**Sai lầm thường gặp:** đặt mỗi “mục trên Home” thành 1 `features/xxx` → thực chất là **page slice**, không phải domain ngang.

## Redux trong `app/store.ts`

```ts
{
  auth,              // features/auth
  user,              // features/user — domain ngang
  homeServiceHealth, // pages/home/model — chỉ Home
}
```

### User — API gọi trực tiếp tại page (không Thunk, không hook)

```ts
// Trong UserInfoSection.tsx hoặc UserPage.tsx
dispatch(setLoading());
const data = await fetchUserFromApi();
dispatch(setUser(data));
```

| File | Vai trò |
|------|---------|
| `userApi.ts` | Hàm HTTP `fetchUserFromApi()` |
| `userSlice.ts` | `setLoading`, `setUser`, `setFailed` — page tự dispatch |
| Page | Gọi API + dispatch + `useAppSelector` đọc state |

Khi ping health cần ở Dashboard → **promote** `pages/home/model` → `features/service-health/`.

## SCSS

- Page: `HomePage.module.scss`
- Page component: `pages/home/components/HomeHeader/*.module.scss`
- Page section: `pages/home/sections/UserInfoSection/*.module.scss`
- Feature **không** chứa UI/scss (chỉ TS logic)
