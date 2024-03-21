import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  page.on('dialog', dialog => dialog.accept());
  await page.goto('https://localhost:7237/');
  await page.getByRole('link', { name: 'Login' }).click();
  await page.getByPlaceholder('nome@email.com').fill('admin@melodycircle.pt');
  await page.getByPlaceholder('Palavra-passe').fill('Password-123');
  await page.getByRole('button', { name: 'Log in' }).click();
  await page.getByRole('link', { name: 'Colab.' }).click();
  await page.getByRole('main').locator('div').filter({ hasText: 'Collaboration 2 dd-mm-aaaa' }).getByRole('link').click();
  await page.getByRole('link', { name: 'Home.' }).click();
});