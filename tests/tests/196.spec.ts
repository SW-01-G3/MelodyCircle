import { test, expect } from "@playwright/test";
import randomNumber from "../helper/randomNumber";

let lastPostAdded: string;

test("test", async ({ page }) => {
  page.on("dialog", (dialog) => dialog.accept());
  await page.goto("https://localhost:7237/");
  await page.getByRole("link", { name: "Login" }).click();
  await page
    .getByPlaceholder("username@email.com or iamuser123")
    .fill("admin@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  for (let cnt = 0; cnt < 8; cnt++) {
    lastPostAdded = "Forum" + randomNumber(2, 999).toString();
    await page.getByRole("link", { name: "+" }).click();
    await page.getByLabel("Title").click();
    await page.getByLabel("Title").fill(lastPostAdded);
    await page
      .locator("#editor div")
      .filter({ hasText: "Hello, World" })
      .click();
    await page
      .locator("#editor div")
      .filter({ hasText: "Hello, World" })
      .fill("Forum1");
    await page.getByRole("button", { name: "Criar" }).click();
  }
  await page.mouse.wheel(0, 15000);
  await page.getByRole("link", { name: "Ver comentários" }).nth(7).click();
});
