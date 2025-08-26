import "./App.css";
import { Box, CssBaseline } from "@mui/material";
import { LoginPage, VaultPage } from "./pages";
import "react-toastify/dist/ReactToastify.css";
import { ConfirmProvider } from "material-ui-confirm";
import { ThemeProvider } from "./contexts/ThemeContext";
import { BrowserRouter, Route, Routes } from "react-router-dom";

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
        </ConfirmProvider>
      </ThemeProvider>
    </Box>
  );
}

export default App;
