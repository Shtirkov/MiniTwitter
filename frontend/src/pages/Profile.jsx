import { Box, Heading, VStack, Text } from "@chakra-ui/react";

export default function Profile() {
    const posts = [
        { id: 1, content: "Моят първи пост 🐦" },
        { id: 2, content: "Днес научих React Router 🚀" },
    ];

    return (
        <Box
            bg="gray.900"
            color="white"
            minH="100vh"
            w="100vw"        // 🔑 пълна ширина
            pt="80px"        // за navbar
            px={6}
        >
            <Heading mb={6}>My Profile</Heading>

            <VStack spacing={6} align="stretch">
                {posts.map((post) => (
                    <Box
                        key={post.id}
                        bg="gray.800"
                        p={4}
                        rounded="md"
                        shadow="md"
                    >
                        <Text>{post.content}</Text>
                    </Box>
                ))}
            </VStack>
        </Box>
    );
}
