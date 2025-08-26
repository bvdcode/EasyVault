import axios from "axios";
import { VaultData } from "../types";

const API_BASE_URL = "/api/v1";

export default class VaultApiService {
  /**
   * Retrieves vault data by password
   * @param password - The password to access the vault
   * @returns Promise<VaultData[]> - The vault data array
   * * @throws Error if the request fails
   */
  static async getVaultData(password: string): Promise<VaultData[]> {
    const response = await axios.get<VaultData[]>(
      `${API_BASE_URL}/vault/${encodeURIComponent(password)}`,
    );
    if (response.status !== 200) {
      throw new Error(response.statusText);
    }
    if (!Array.isArray(response.data)) {
      throw new Error("Invalid response format: expected an array");
    }
    return response.data;
  }

  /**
   *  Saves vault data with the given password
   * @param password - The password to access the vault
   * @param data - The vault data to save
   * * @returns Promise<void> - Resolves when the data is saved successfully
   * * @throws Error if the request fails
   * * This method will overwrite existing data for the given password
   * * Note: Ensure that the password is securely handled and not logged or exposed
   */
  static async saveVaultData(
    password: string,
    data: VaultData[],
  ): Promise<void> {
    if (!Array.isArray(data)) {
      throw new Error("Data must be an array of VaultData");
    }
    if (data.length === 0) {
      throw new Error("Data array cannot be empty");
    }
    const response = await axios.post(
      `${API_BASE_URL}/vault/${encodeURIComponent(password)}`,
      data,
    );
    if (response.status !== 200) {
      throw new Error(response.statusText);
    }
  }
}
