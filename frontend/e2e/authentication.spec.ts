import { expect, test } from '@playwright/test';

test('redirects an anonymous user from a protected page to login', async ({ page }) => {
  await page.goto('/dashboard');

  await expect(page).toHaveURL(/\/login\?returnUrl=%2Fdashboard$/);
  await expect(page.getByRole('heading', { name: 'Welcome back' })).toBeVisible();
});

test('requires valid tenant, email, and password values before login', async ({ page }) => {
  await page.goto('/login');
  const submit = page.getByRole('button', { name: 'Sign in securely' });

  await expect(submit).toBeDisabled();
  await page.getByLabel('Hospital tenant ID').fill('not-a-guid');
  await page.getByLabel('Email address').fill('invalid-email');
  await page.getByLabel('Password').fill('short');
  await expect(submit).toBeDisabled();

  await page.getByLabel('Hospital tenant ID').fill('11111111-1111-1111-1111-111111111111');
  await page.getByLabel('Email address').fill('admin@hospital.local');
  await page.getByLabel('Password').fill('ValidPassword123!');
  await expect(submit).toBeEnabled();
});

test('logs in against the live backend when E2E credentials are supplied', async ({ page }) => {
  const tenantId = process.env['E2E_TENANT_ID'];
  const email = process.env['E2E_ADMIN_EMAIL'];
  const password = process.env['E2E_ADMIN_PASSWORD'];
  test.skip(!tenantId || !email || !password, 'Live backend credentials were not supplied.');

  await page.goto('/login');
  await page.getByLabel('Hospital tenant ID').fill(tenantId!);
  await page.getByLabel('Email address').fill(email!);
  await page.getByLabel('Password').fill(password!);
  await page.getByRole('button', { name: 'Sign in securely' }).click();

  await expect(page).toHaveURL(/\/dashboard$/);
  await expect(page.getByRole('heading', { name: /welcome back/i })).toBeVisible();
  await expect(page.getByText(email!, { exact: true })).toBeVisible();
});
