import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { Box } from "@chakra-ui/react";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Feed from "./pages/Feed";
import Profile from "./pages/Profile";
import Navbar from "./components/Navbar";
import ProtectedRoute from "./components/ProtectedRoute";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* redirect към login */}
        <Route path="/" element={<Navigate to="/login" />} />

        {/* публични */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* защитени */}
        <Route
          path="/feed"
          element={
            <ProtectedRoute>
              <Box minH="100vh" w="100vw">
                <Navbar />
                <Feed />
              </Box>
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <Box minH="100vh" w="100vw">
                <Navbar />
                <Profile />
              </Box>
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
