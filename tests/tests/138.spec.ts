import { test, expect } from "@playwright/test";

/* Eduardo Andrade, André Nunes */
test("test", async ({ page }) => {
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("username@email.com or").fill("mod2");
  await page.getByPlaceholder("Palavra-Passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  await page.getByRole("link", { name: "Procurar." }).click();
  await page.getByRole("link", { name: "admin1" }).click();
  await page.getByRole("button").nth(1).click();
});
