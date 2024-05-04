import { test, expect } from "@playwright/test";

/* Eduardo Andrade, André Nunes */
test("test", async ({ page }) => {
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("username@email.com or").click();
  await page.getByPlaceholder("username@email.com or").fill("mod1");
  await page.getByPlaceholder("Palavra-Passe").click();
  await page.getByPlaceholder("Palavra-Passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  await page.getByRole("link", { name: "Procurar." }).click();
  await page.getByRole("combobox").selectOption("User");
  await page.getByPlaceholder("Pesquisar").click();
  await page.getByPlaceholder("Pesquisar").fill("professor1");
  await page.getByPlaceholder("Pesquisar").press("Enter");
  await page
    .getByRole("heading", { name: "Resultados da Pesquisa dos Utilizadores" })
    .click();
});
