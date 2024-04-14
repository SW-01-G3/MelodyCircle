import { test, expect } from "@playwright/test";

test("test", async ({ page }) => {
  page.on("dialog", (dialog) => dialog.accept());
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("nome@email.com").fill("admin@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "Colab." }).click();
  await page.getByRole("link", { name: "+" }).click();
  await page.getByLabel("Título:").click();
  await page.getByLabel("Título:").fill("a");
  await page.getByText("Hello, World").click();
  await page
    .locator("#editor div")
    .filter({ hasText: "Hello, World" })
    .fill("Hello, Worlda");
  await page.getByLabel("Número Máximo de Utilizadores:").click();
  await page.getByLabel("Número Máximo de Utilizadores:").fill("2");
  await page.getByLabel("Senha de Acesso:").click();
  await page.getByLabel("Senha de Acesso:").fill("123123");
  await page.getByLabel("Foto").click();
  await page.getByLabel("Foto").setInputFiles("transferir.jfif");
  await page.getByRole("button", { name: "Adicionar" }).click();
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Colab." }).click();
  await page.getByRole("link", { name: "Abrir Painel de Arranjo" }).click();
  await page.getByRole("link", { name: "Colab." }).click();
});
