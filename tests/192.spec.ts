import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  // Recording...
  page.on('dialog', dialog => dialog.accept());
  await page.goto('https://localhost:7237/');
  await page.getByRole('link', { name: 'Login' }).click();
  await page.getByPlaceholder('nome@email.com').fill('admin@melodycircle.pt');
  await page.getByPlaceholder('Palavra-passe').fill('Password-123');
  await page.getByRole('button', { name: 'Log in' }).click();
  await page.getByRole('link', { name: 'Home.' }).click();
  await page.getByRole('link', { name: 'Notif.' }).click();
  await page.getByRole('link', { name: 'Admin.' }).click();
  await page.getByRole('link', { name: 'Home.' }).first().click();
  await page.getByRole('link', { name: 'Procurar.' }).click();
  await page.getByRole('link', { name: 'Tuto.' }).click();
  await page.getByRole('link', { name: 'Colab.' }).click();
  await page.getByRole('button', { name: 'Home. Drop.' }).click();
  await page.getByRole('button', { name: 'Pefil'}).click();
  await page.getByRole('link', { name: 'Drop.' }).click();
  await page.getByRole('link', { name: 'Home.' }).click();
});