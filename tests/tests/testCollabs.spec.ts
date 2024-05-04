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
  await page.getByRole("link", { name: "Colab." }).click();
  await page.getByRole("link", { name: "+" }).click();
  await page.getByLabel("Título:").click();
  await page.getByLabel("Título:").fill("a");
  await page.getByLabel("Número Máximo de Utilizadores:").fill("2");
  await page.getByLabel("Palavra-Passe").fill("1234");
  await page.locator('input[name="photo"]').setInputFiles("./images/bird.jpg");
  await page.locator('input[name="photo"]').click;
  await page.getByRole("button", { name: "Adicionar" }).click();
  await page
    .getByRole("link", { name: "Abrir Painel de Arranjo" })
    .first()
    .click();
});
