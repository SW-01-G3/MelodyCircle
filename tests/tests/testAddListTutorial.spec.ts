import { test, expect } from "@playwright/test";
import randomNumber from "../helper/randomNumber";

let tutorialTitle: string =
  "UmTutorialDeTeste" + randomNumber(0, 9999).toString();

test("Testar listagem e adição de tutoriais.", async ({ page }) => {
  await page.goto("https://melodycircle.azurewebsites.net/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("nome@email.com").fill("mod@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("link", { name: "Tuto." }).click();
  await page.getByRole("link", { name: "+" }).click();
  await page.getByLabel("Title").fill(tutorialTitle);
  await page.getByLabel("Description").fill("Com descrição");
  await page
    .locator('input[name="photo"]')
    .setInputFiles("Audi_R18_TDI_-_Le_Mans_test_2011.jpg");
  await page.getByRole("button", { name: "Adicionar" }).click();
  await expect(
    page.getByRole("heading", { name: tutorialTitle }).first()
  ).toBeVisible();
});
