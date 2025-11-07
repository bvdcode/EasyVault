import "./App.css";
import { Box, CssBaseline, Fab } from "@mui/material";
import { LoginPage, VaultPage } from "./pages";
import "react-toastify/dist/ReactToastify.css";
import { ConfirmProvider } from "material-ui-confirm";
import { ThemeProvider } from "./contexts/ThemeContext";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { GitHub } from "@mui/icons-material";

function App() {
  return (
    <Box className="app">
      <ThemeProvider>
        <ConfirmProvider>
          <BrowserRouter basename="/">
            <Routes>
              <Route path="/vault" element={<VaultPage />} />
              <Route path="*" element={<LoginPage />} />
            </Routes>
          </BrowserRouter>
          <CssBaseline enableColorScheme={true} />
          <Fab
            color="primary"
            aria-label="add"
            sx={{ position: "fixed", bottom: 16, right: 16 }}
            href="https://github.com/bvdcode/EasyVault"
            target="_blank"
            rel="noopener noreferrer"
          >
            <GitHub />
          </Fab>
        </ConfirmProvider>
      </ThemeProvider>
    </Box>
  );
}

export default App;
