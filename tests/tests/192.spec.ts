import { test, expect } from "@playwright/test";

test("test", async ({ page }) => {
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("username@email.com or").fill("mod1");
  await page.getByPlaceholder("Palavra-Passe").fill("Password-123");
  await page.getByPlaceholder("Palavra-Passe").press("Enter");
  await page.getByRole("link", { name: "Procurar." }).click();
  await page.getByPlaceholder("Pesquisar").fill("admin1");
  await page.getByPlaceholder("Pesquisar").press("Enter");
  await page.getByRole("link", { name: "admin1" }).click();
  await page.getByRole("heading", { name: "admin1", exact: true }).click();
});
