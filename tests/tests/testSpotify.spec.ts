import { test, expect } from "@playwright/test";

test("Testar spotify", async ({ page }) => {
  await page.goto("https://melodycircle.azurewebsites.net/");
  await page.getByRole("link", { name: "Login" }).click();
  await page.getByPlaceholder("nome@email.com").fill("mod@melodycircle.pt");
  await page.getByPlaceholder("Palavra-passe").fill("Password-123");
  await page.getByRole("button", { name: "Log in" }).click();
  await page.getByRole("button", { name: "Home. Drop." }).click();
  await page.getByRole("button", { name: "Perfil" }).click();
  await page
    .getByPlaceholder("https://open.spotify.com/")
    .first()
    .fill(
      "https://open.spotify.com/track/7o2AeQZzfCERsRmOM86EcB?si=12939d40badc47fc"
    );
  await page.getByRole("button", { name: "Adicionar" }).click();
  const page1Promise = page.waitForEvent("popup");
  await expect(
    await page.frameLocator("iframe").getByRole("link", { name: "Xtal" })
  ).toBeVisible();
});
