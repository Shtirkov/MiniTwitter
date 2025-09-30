import { Box, Button, Heading, Input, VStack, Text, Link, useToast } from "@chakra-ui/react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";


export default function Register() {
    const [email, setEmail] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const toast = useToast();
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch("http://localhost:5064/api/Auth/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, email, password }),
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message);
            }

            toast({
                title: "Registration successful",
                status: "success",
                duration: 2000,
                isClosable: true,
            });

            navigate("/login");
        } catch (err) {
            toast({
                title: "Registration failed",
                description: err.message,
                status: "error",
                duration: 3000,
                isClosable: true,
            });
        }
    };

    return (
        <Box
            position="relative"
            height="100vh"
            width="100vw"
            display="flex"
            alignItems="center"
            justifyContent="center"
            backgroundImage="url('https://images.pexels.com/photos/1015568/pexels-photo-1015568.jpeg')"
            backgroundSize="cover"
            backgroundPosition="center"
        >
            <Box
                position="absolute"
                top={0}
                left={0}
                right={0}
                bottom={0}
                bg="blackAlpha.700"
            />

            <Box
                position="relative"
                bg="gray.800"
                p={10}
                rounded="xl"
                shadow="2xl"
                width={["90%", "400px"]}
                border="1px solid"
                borderColor="gray.700"
            >
                <Heading mb={8} textAlign="center" color="white">
                    MiniTwitter
                </Heading>
                <form onSubmit={handleRegister}>
                    <VStack spacing={5}>
                        <Input
                            placeholder="Email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            bg="gray.700"
                            border="none"
                            color="white"
                            _placeholder={{ color: "gray.400" }}
                            _focus={{ bg: "gray.600" }}
                        />
                        <Input
                            placeholder="Username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            bg="gray.700"
                            border="none"
                            color="white"
                            _placeholder={{ color: "gray.400" }}
                            _focus={{ bg: "gray.600" }}
                        />
                        <Input
                            placeholder="Password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            bg="gray.700"
                            border="none"
                            color="white"
                            _placeholder={{ color: "gray.400" }}
                            _focus={{ bg: "gray.600" }}
                        />
                        <Button colorScheme="blue" width="full" type="submit" size="lg">
                            Register
                        </Button>
                    </VStack>
                </form>
                <Text mt={6} textAlign="center" color="gray.400">
                    Already have an account?{" "}
                    <Link color="blue.300" href="/login">
                        Login
                    </Link>
                </Text>
            </Box>
        </Box>
    );
}
