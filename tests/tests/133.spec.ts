import { test, expect } from "@playwright/test";

test("test", async ({ page }) => {
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page
    .getByPlaceholder("username@email.com or iamuser123")
    .fill("admin@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  await page.getByRole("link", { name: "Notif." }).click();
  await page.getByRole("link", { name: "Procurar." }).click();
  await page.getByRole("link", { name: "mod2" }).click();
  await page.getByRole("button").nth(3).click();
  await page
    .locator("#starRatingForm > .star-rating > img:nth-child(5)")
    .click();
  await page.getByRole("button", { name: "Avaliar" }).click();
});
