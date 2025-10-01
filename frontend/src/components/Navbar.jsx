import { Flex, Box, Button, Heading, useToast } from "@chakra-ui/react";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";

export default function Navbar() {

    const navigate = useNavigate();
    const toast = useToast();

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/login");

        toast({
            title: "Logged out",
            status: "info",
            duration: 3000,
            isClosable: true,
        });

    }

    return (
        <Flex
            as="nav"
            bg="gray.800"
            color="white"
            p={4}
            align="center"
            shadow="md"
            position="fixed"
            top={0}
            left={0}
            right={0}
            zIndex={10}
        >
            <Heading size="md">MiniTwitter</Heading>
            <Box ml="auto">
                <Button as={Link} to="/feed" variant="ghost" color="white">
                    Home
                </Button>
                <Button as={Link} to="/profile" variant="ghost" color="white">
                    Profile
                </Button>
                <Button onClick={handleLogout} variant="ghost" color="red.300">
                    Logout
                </Button>
            </Box>
        </Flex>
    );
}
