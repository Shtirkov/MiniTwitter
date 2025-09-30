import { Box, Button, Heading, Textarea, VStack, Text } from "@chakra-ui/react";
import { useState } from "react";

export default function Feed() {
    const [posts, setPosts] = useState([
        { id: 1, user: "Alice", content: "Hello world!" },
        { id: 2, user: "Bob", content: "First post here 🚀" },
    ]);
    const [newPost, setNewPost] = useState("");

    const handlePost = () => {
        if (!newPost) return;
        const newEntry = { id: posts.length + 1, user: "Me", content: newPost };
        setPosts([newEntry, ...posts]);
        setNewPost("");
    };

    return (
        <Box
            bg="gray.900"
            color="white"
            minH="100vh"
            w="100vw"       // 🔑 пълна ширина на екрана
            pt="80px"       // за navbar
            px={6}
        >
            <Heading mb={6}>Feed</Heading>

            <VStack spacing={4} mb={8} align="stretch">
                <Textarea
                    placeholder="What's happening?"
                    value={newPost}
                    onChange={(e) => setNewPost(e.target.value)}
                    bg="gray.800"
                    border="none"
                    _placeholder={{ color: "gray.400" }}
                />
                <Button colorScheme="blue" alignSelf="flex-end" onClick={handlePost}>
                    Post
                </Button>
            </VStack>

            <VStack spacing={6} align="stretch">
                {posts.map((post) => (
                    <Box key={post.id} bg="gray.800" p={4} rounded="md" shadow="md">
                        <Text fontWeight="bold">{post.user}</Text>
                        <Text>{post.content}</Text>
                    </Box>
                ))}
            </VStack>
        </Box>
    );
}
