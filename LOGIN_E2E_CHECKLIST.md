# Login E2E Checklist

## 0. Preconditions
- Docker Desktop is running and healthy.
- Root `.env` contains secure values:
  - `AUTH_ADMIN_PASSWORD=<strong-password>`
  - `AUTH_USER_PASSWORD=<strong-password>`
  - `JWT_SECRET=<shared-secret-if-HMAC-mode>`
  - `IAM_VALIDATE_URL=<iam-validation-endpoint-if-required>`
   - `SSO_PRIVATE_KEY=<rsa-private-key-for-gateway-signing>`
- Ports are free: `5010`, `5050`, `5000`, `5001`, `5173`.

## 1. Start stack
1. Run: `docker-compose up -d --build`
2. Confirm containers are healthy:
   - `docker-compose ps`
   - `docker-compose logs -f auth-server`
   - `docker-compose logs -f gateway`

Expected:
- `auth-server` healthy on `http://localhost:5010`.
- `gateway` healthy on `http://localhost:5050/health`.

## 2. Verify AuthServer basic behavior
1. Open `http://localhost:5010/account/login`
2. Attempt login with invalid credentials.
3. Attempt login with configured credentials.

Expected:
- Invalid credentials show error.
- Valid credentials create cookie session.
- Redirect only to local path (no external redirect).

## 3. Verify token issuance flow
1. Trigger login flow from React app (`http://localhost:5173`).
2. Complete IAM/Auth login.
3. Confirm callback stores token and removes token from URL.

Expected:
- Browser lands on app home page.
- Gateway generates SSO sign-in URL (FE does not sign client-side).
- `access_token` is not left in query/hash after callback handling.
- App authenticated state is true.

## 4. Verify Gateway protected routes (fail-open prevention)
1. Call protected route without token:
   - `GET http://localhost:5050/api/base/api/...`
2. Call protected route with malformed token.
3. Call protected route with valid token.

Expected:
- No token => `401`.
- Invalid token => `401`.
- Valid token => proxied success response.

## 5. Verify backend JWT policy modes
### Mode A: Authority/JWKS
1. Set `JsonWebTokenKeys:Authority` to AuthServer authority.
2. Keep `UseExternalValidation` as needed.
3. Restart service-base/service-upload.

Expected:
- Signature validation uses authority metadata/JWKS.
- Invalid signature token is rejected before external validation.

### Mode B: IAM contract + symmetric key
1. Keep `Authority` empty.
2. Set `IssuerSigningKey` and issuer/audience policy per contract.
3. Keep `UseExternalValidation=true` if IAM endpoint is mandatory.

Expected:
- Cryptographic token validation still enforced.
- External IAM validation acts as an additional gate.

## 6. Negative security checks
1. Set `UseExternalValidation=false` in Gateway and retry protected route without token.
2. Try external returnUrl value on login.

Expected:
- Gateway still returns `401` for unauthenticated requests.
- AuthServer ignores external returnUrl and redirects to `/`.

## 7. Regression checks
1. Upload flow still works via gateway route.
2. Service-base protected controllers still require auth.
3. Logout clears session/token behavior as expected.

## 8. Troubleshooting
- If `docker-compose` returns API 500 for Docker pipe, restart Docker Desktop and check engine mode.
- If login always fails, verify `AUTH_ADMIN_PASSWORD`/`AUTH_USER_PASSWORD` are set and not empty.
- If JWT fails unexpectedly, inspect service logs for `JWT validation failed` warnings.
