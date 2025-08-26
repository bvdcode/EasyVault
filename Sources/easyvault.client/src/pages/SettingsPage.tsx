import React from "react";
import { useTranslation } from "react-i18next";

const SettingsPage: React.FC = () => {
  const { t } = useTranslation();

  return <>{t("settings.title")}</>;
};

export default SettingsPage;
