import { VaultData } from "../types";
import { useTranslation } from "react-i18next";
import React, { useEffect, useState } from "react";
import { Settings, VaultList } from "../components";
import { Box, Paper, Tab, Tabs } from "@mui/material";
import { useLocation, useNavigate } from "react-router-dom";

const VaultPage: React.FC = () => {
  const { t } = useTranslation();
  const location = useLocation();
  const navigate = useNavigate();
  const password = location.state?.password;
  const [selectedTab, setSelectedTab] = useState<number>(0);
  const [vaultData, setVaultData] = useState<VaultData[]>([]);

  useEffect(() => {
    if (!password) {
      navigate("/login");
    }
  }, [navigate, password]);

  const handleImport = (data: VaultData[]) => {
    setVaultData(data);
    // Switch to vault list tab to show imported data
    setSelectedTab(0);
  };

  return (
    <Paper
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "flex-start",
        margin: "auto",
        width: "100%",
        height: "100%",
      }}
    >
      <Tabs
        value={selectedTab}
        onChange={(_, newValue) => setSelectedTab(newValue)}
        aria-label="basic tabs example"
      >
        <Tab label={t("vaultPage.tabs.list")} />
        <Tab label={t("vaultPage.tabs.settings")} />
      </Tabs>
      <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        justifyContent="flex-start"
        width="100%"
        height="100%"
        padding={2}
      >
        {selectedTab === 0 && (
          <VaultList
            password={password}
            vaultData={vaultData}
            onVaultDataChange={setVaultData}
          />
        )}
        {selectedTab === 1 && (
          <Settings vaultData={vaultData} onImport={handleImport} />
        )}
      </Box>
    </Paper>
  );
};

export default VaultPage;
