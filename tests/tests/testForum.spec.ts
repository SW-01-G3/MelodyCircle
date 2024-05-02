import { test, expect } from "@playwright/test";

test("test", async ({ page }) => {
  page.on("dialog", (dialog) => dialog.accept());
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("nome@email.com").fill("admin@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "+" }).click();
  await page.getByLabel("Title").click();
  await page.getByLabel("Title").fill("Forum 1");
  await page.locator("#editor div").filter({ hasText: "Hello, World" }).click();
  await page
    .locator("#editor div")
    .filter({ hasText: "Hello, World" })
    .fill("Forum1");
  await page.getByRole("button", { name: "Create" }).click();
  await page.getByRole("link", { name: "Ver comentários" }).first().click();
  await page.locator("#Content").click();
  await page.locator("#Content").click();
  await page.locator("#Content").fill("ola");
  await page.getByRole("button", { name: "Add Comment" }).click();
  await page.getByRole("link", { name: "Home." }).click();
  await page.locator(".btn").first().click();
});
