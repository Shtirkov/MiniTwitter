import { Flex, Box, Button, Heading } from "@chakra-ui/react";
import { Link } from "react-router-dom";

export default function Navbar() {
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
                <Button as={Link} to="/login" variant="ghost" color="red.300">
                    Logout
                </Button>
            </Box>
        </Flex>
    );
}
