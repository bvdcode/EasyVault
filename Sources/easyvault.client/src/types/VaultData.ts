export default interface VaultData {
  keyId: string; // Guid in C# maps to string in TypeScript
  appName: string;
  values: Record<string, string>; // Dictionary<string, string> in C#
  allowedAddresses: string[];
  allowedUserAgents: string[];
}
