import { Box, Button, Heading, Input, VStack, Text, Link, useToast } from "@chakra-ui/react";
import { useState } from "react";
import loginBackground from '../assets/loginBackground.png'
import { useNavigate } from "react-router-dom";


export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const toast = useToast();
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch("http://localhost:5064/api/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password }),
            });

            const data = await response.json();

            if (!response.ok) {
                if (data.error) {
                    if (data.details !== undefined) {
                        throw new Error(data.details);
                    }
                    else throw new Error(data.error);
                }
                if (data.errors) {
                    const messages = Object.values(data.errors).flat();
                    throw new Error(messages.join("\n"));
                }
                throw new Error("Login failed");
            }

            localStorage.setItem("username", data.username);
            localStorage.setItem("token", data.token);

            toast({
                title: "Login successful",
                status: "success",
                duration: 3000,
                isClosable: true,
            });

            navigate("/feed");
        } catch (err) {
            toast({
                title: "Login failed",
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
            backgroundImage={loginBackground}
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
                <form onSubmit={handleLogin}>
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
                            Sign In
                        </Button>
                    </VStack>
                </form>
                <Text mt={6} textAlign="center" color="gray.400">
                    Don’t have an account?{" "}
                    <Link color="blue.300" href="/register">
                        Register
                    </Link>
                </Text>
            </Box>
        </Box>
    );
}
