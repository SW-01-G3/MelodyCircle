import { test, expect } from "@playwright/test";

/* Eduardo Andrade, André Nunes */
test("test", async ({ page }) => {
  page.on("dialog", (dialog) => dialog.accept());
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page
    .getByPlaceholder("username@email.com or iamuser123")
    .fill("admin@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  await page.getByRole("link", { name: "Admin." }).click();
  await page.getByRole("button").nth(1).click();
  await page
    .getByRole("link", { name: "Ver Estatísticas de Utilizadores" })
    .click();
  await page.getByRole("link", { name: "Admin." }).click();
  await page.getByRole("button").nth(1).click();
  await page
    .getByRole("link", { name: "Ver Estatísticas de Tutoriais" })
    .click();
  await page.getByRole("link", { name: "Admin." }).click();
  await page.getByRole("button").nth(1).click();
  await page.getByRole("link", { name: "Ver Estatísticas de Passos" }).click();
  await page.getByRole("link", { name: "Admin." }).click();
  await page.getByRole("button").nth(1).click();
  await page
    .getByRole("link", { name: "Ver Estatísticas de Colaborações" })
    .click();
  await page.getByRole("link", { name: "Admin." }).click();
  await page.getByRole("button").nth(1).click();
});
