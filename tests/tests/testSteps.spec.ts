import { test, expect } from "@playwright/test";
import randomNumber from "../helper/randomNumber";

let stepText: string = "TestStep" + randomNumber(0, 9999).toString();

test("Adicionar passo a um tutorial, listar passos, editar passo e apagar passo.", async ({
  page,
}) => {
  await page.goto("https://melodycircle.azurewebsites.net/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("nome@email.com").fill("mod@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "Tuto." }).click();
  await page.getByRole("link", { name: "Abrir" }).first().click();
  await page.getByRole("link", { name: "+" }).click();
  await page.getByLabel("Title").fill(stepText);
  await page
    .locator("#editor div")
    .filter({ hasText: "Hello, World" })
    .fill(stepText);
  await page.getByLabel("Order", { exact: true }).fill("3");
  await page.getByRole("button", { name: "Adicionar" }).click();
  await expect(
    await page
      .locator("section")
      .filter({ hasText: stepText })
      .getByRole("paragraph")
  ).toBeVisible();
  await page.getByRole("link", { name: "Edit step." }).last().click();
  await page
    .locator("#editor div")
    .filter({ hasText: stepText })
    .fill(stepText + stepText);
  await page.getByRole("button", { name: "Salvar" }).click();
  await expect(
    await page
      .locator("section")
      .filter({ hasText: stepText + stepText })
      .getByRole("paragraph")
  ).toBeVisible();
  await page.getByRole("link", { name: "Delete step." }).last().click();
  await page.getByRole("button", { name: "Sim" }).click();
  await expect(
    await page
      .locator("section")
      .filter({ hasText: stepText + stepText })
      .getByRole("paragraph")
  ).not.toBeVisible();
});
