import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  // Recording...
page.on('dialog', dialog => dialog.accept());
await page.goto('https://localhost:7237/');
await page.getByRole('link', { name: 'Login' }).click();
await page.getByPlaceholder('nome@email.com').fill('admin@melodycircle.pt');
await page.getByPlaceholder('Palavra-passe').fill('Password-123');
await page.getByRole('button', { name: 'Log in' }).click();
await page.getByRole('link', { name: 'Notif.' }).click();
await page.getByRole('row', { name: 'Moderator2 mod2 Other 22/01/' }).getByRole('link').first().click();
await page.getByRole('button').nth(1).click();
await page.getByRole('button').nth(1).click();
await page.getByRole('link', { name: 'Drop.' }).click();
await page.getByRole('link', { name: 'Home.' }).click();
});