import {
  Box,
  Button,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  SelectChangeEvent,
  Stack,
  Switch,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAppTheme } from "../contexts/ThemeContext";
import { Brightness4, Brightness7 } from "@mui/icons-material";

const Settings: React.FC = () => {
  const navigate = useNavigate();
  const { t, i18n } = useTranslation();
  const { isDarkMode, toggleTheme } = useAppTheme();

  const handleLanguageChange = (event: SelectChangeEvent<string>) => {
    i18n.changeLanguage(event.target.value);
  };

  return (
    <Paper sx={{ p: 3, width: "100%", mx: "auto" }}>
      <Stack spacing={4} mt={3} maxWidth={400} mx="auto">
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <InputLabel id="language-select-label">
            {t("settings.darkMode")}
          </InputLabel>
          <Box
            display="flex"
            alignItems="center"
            justifyContent="center"
            sx={{ width: "auto" }}
          >
            <Brightness7
              sx={{
                mr: 2,
                color: isDarkMode ? "text.disabled" : "warning.light",
              }}
            />
            <Switch
              id="theme-switch"
              checked={isDarkMode}
              onChange={toggleTheme}
              name="themeMode"
              color="primary"
            />
            <Brightness4
              sx={{ ml: 2, color: isDarkMode ? "info.light" : "text.disabled" }}
            />
          </Box>
        </Box>

        <Box display="flex" alignItems="center" justifyContent="space-between">
          <InputLabel id="language-select-label">
            {t("settings.language")}
          </InputLabel>
          <Box display="flex" alignItems="center" justifyContent="center">
            <Select
              labelId="language-select-label"
              id="language-select"
              value={i18n.language.substring(0, 2)}
              onChange={handleLanguageChange}
              label={t("settings.language")}
              variant="standard"
            >
              <MenuItem value="en">English</MenuItem>
              <MenuItem value="ru">Русский</MenuItem>
            </Select>
          </Box>
        </Box>

        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Button
            onClick={() => navigate("/login")}
            variant="outlined"
            fullWidth
          >
            {t("settings.logout")}
          </Button>
        </Box>
      </Stack>
    </Paper>
  );
};

export default Settings;
