import { test, expect } from "@playwright/test";

test("Testar spotify", async ({ page }) => {
  await page.goto("https://localhost:7237");
  await page.getByRole("link", { name: "Login" }).click();
  await page
    .getByPlaceholder("username@email.com or iamuser123")
    .fill("mod@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Conecte-se" }).click();
  await page.getByRole("button", { name: "Home. Drop." }).click();
  await page.getByRole("button", { name: "Perfil" }).click();
  await page.getByRole("link", { name: "+" }).click();
  await page
    .locator('input[name="musicUri"]')
    .fill(
      "https://open.spotify.com/track/7o2AeQZzfCERsRmOM86EcB?si=12939d40badc47fc"
    );

  await page.getByRole("button", { name: "Adicionar" }).click();
  await expect(
    await page.frameLocator("iframe").getByRole("link", { name: "Xtal" })
  ).toBeVisible();
});
