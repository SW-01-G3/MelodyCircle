import { test, expect } from "@playwright/test";
import randomNumber from "../helper/randomNumber";

let tutorialTitle: string =
  "UmTutorialDeTeste" + randomNumber(0, 9999).toString();

test("Testar listagem e adição de tutoriais.", async ({ page }) => {
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page
    .getByPlaceholder("username@email.com or iamuser123")
    .fill("mod@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  await page.getByRole("link", { name: "Tuto." }).click();
  await page.getByRole("link", { name: "+" }).click();
  await page.getByLabel("Title").fill(tutorialTitle);
  await page.locator('input[name="photo"]').setInputFiles("./images/bird.jpg");
  await page.locator('input[name="photo"]').click;
  await page.getByRole("button", { name: "Adicionar" }).click();
  await expect(page).toHaveURL("https://localhost:7237/Tutorial/EditMode");
  await expect(
    page.getByRole("heading", { name: tutorialTitle }).first()
  ).toBeVisible();
});
