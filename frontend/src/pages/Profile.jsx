import {
    Box,
    Button,
    Flex,
    Heading,
    Text,
    VStack,
    HStack,
    Input,
    useToast
} from "@chakra-ui/react";
import { useEffect, useState } from "react";

const API = "http://localhost:5064/api";

export default function Profile() {
    const [posts, setPosts] = useState([]);
    const [friends, setFriends] = useState([]);
    const [requests, setRequests] = useState([]);
    const [newFriend, setNewFriend] = useState("");
    const token = localStorage.getItem("token");
    const currentUser = localStorage.getItem("username");
    const toast = useToast();

    // зареждане на постовете
    useEffect(() => {
        const fetchPosts = async () => {
            try {
                const res = await fetch(`${API}/posts/user/${currentUser}`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!res.ok) throw new Error(await res.text());
                const data = await res.json();
                setPosts(data.items || []);
            } catch (err) {
                console.error("Error loading user posts:", err);
            }
        };
        fetchPosts();
    }, [token, currentUser]);

    // зареждане на приятели
    useEffect(() => {
        const fetchFriends = async () => {
            try {
                const res = await fetch(`${API}/friendships/friends`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!res.ok) throw new Error(await res.text());
                const data = await res.json();
                setFriends(data || []);
            } catch (err) {
                console.error("Error loading friends:", err);
            }
        };
        fetchFriends();
    }, [token]);

    // зареждане на pending friend requests
    useEffect(() => {
        const fetchRequests = async () => {
            try {
                const res = await fetch(`${API}/friendships/pendingRequests`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!res.ok) throw new Error(await res.text());
                const data = await res.json();
                setRequests(data || []);
            } catch (err) {
                console.error("Error loading requests:", err);
            }
        };
        fetchRequests();
    }, [token]);

    const handleSendRequest = async () => {
        if (!newFriend.trim()) return;
        try {
            const res = await fetch(`${API}/friendships/send/${newFriend}`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            setNewFriend("");

            toast({
                title: "Friend request sent!",
                status: "success",
                duration: 2000,
                isClosable: true,
            });
        } catch (err) {
            toast({
                title: "Error sending request",
                description: err.message,
                status: "error",
                duration: 3000,
                isClosable: true,
            });
        }
    };

    // accept friend request
    const handleAccept = async (username) => {
        try {
            const res = await fetch(`${API}/friendships/accept/${username}`, {
                method: "PUT",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            const updated = await res.json();

            setRequests((prev) => prev.filter((r) => r.friendUsername !== username));
            setFriends((prev) => [...prev, updated]);

            toast({
                title: `You are now friends with ${username}!`,
                status: "success",
                duration: 2000,
                isClosable: true,
            });
        } catch (err) {
            toast({
                title: "Error accepting request",
                description: err.message,
                status: "error",
                duration: 3000,
                isClosable: true,
            });
        }
    };

    // reject friend request
    const handleReject = async (username) => {
        try {
            const res = await fetch(`${API}/friendships/reject/${username}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());

            setRequests((prev) => prev.filter((r) => r.friendUsername !== username));

            toast({
                title: `Friend request from ${username} rejected`,
                status: "info",
                duration: 2000,
                isClosable: true,
            });
        } catch (err) {
            toast({
                title: "Error rejecting request",
                description: err.message,
                status: "error",
                duration: 3000,
                isClosable: true,
            });
        }
    };

    // delete пост
    const handleDeletePost = async (id) => {
        try {
            const res = await fetch(`${API}/posts/${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            setPosts((prev) => prev.filter((p) => p.id !== id));
        } catch (err) {
            console.error("Delete post error:", err);
        }
    };

    return (
        <Flex
            bg="gray.900"
            color="white"
            minH="100vh"
            w="100vw"
            pt="80px"
            px={6}
            gap={6}
        >
            {/* Ляв панел - приятели + добавяне */}
            <Box w="20%" bg="gray.800" p={4} rounded="md" shadow="md" h="fit-content">
                <Heading size="md" mb={4}>
                    Friends
                </Heading>
                <HStack mb={4}>
                    <Input
                        placeholder="Add friend..."
                        size="sm"
                        bg="gray.700"
                        value={newFriend}
                        onChange={(e) => setNewFriend(e.target.value)}
                    />
                    <Button size="sm" colorScheme="blue" onClick={handleSendRequest}>
                        Send
                    </Button>
                </HStack>
                <VStack align="stretch" spacing={2}>
                    {friends.length > 0 ? (
                        friends.map((f, idx) => (
                            <Text key={idx} bg="gray.700" p={2} rounded="md">
                                {f.userUsername}
                            </Text>
                        ))
                    ) : (
                        <Text fontSize="sm" color="gray.400">
                            No friends yet
                        </Text>
                    )}
                </VStack>
            </Box>

            {/* Център - постове */}
            <Box flex="1" bg="gray.800" p={4} rounded="md" shadow="md">
                <Heading size="md" mb={4}>
                    My Posts
                </Heading>
                <VStack spacing={4} align="stretch">
                    {posts.length > 0 ? (
                        posts.map((post) => (
                            <Box key={post.id} bg="gray.700" p={3} rounded="md">
                                <Text fontWeight="bold">{post.author}</Text>
                                <Text>{post.content}</Text>
                                <Text fontSize="sm" color="gray.400">
                                    ❤️ {post.likesCount} | 💬 {post.comments?.length || 0}
                                </Text>
                                <Button
                                    size="sm"
                                    colorScheme="red"
                                    mt={2}
                                    onClick={() => handleDeletePost(post.id)}
                                >
                                    Delete Post
                                </Button>
                            </Box>
                        ))
                    ) : (
                        <Text fontSize="sm" color="gray.400">
                            No posts yet
                        </Text>
                    )}
                </VStack>
            </Box>

            {/* Десен панел - friend requests */}
            <Box w="20%" bg="gray.800" p={4} rounded="md" shadow="md" h="fit-content">
                <Heading size="md" mb={4}>
                    Friend Requests
                </Heading>
                <VStack align="stretch" spacing={3}>
                    {requests.length > 0 ? (
                        requests.map((r, idx) => (
                            <Box
                                key={idx}
                                bg="gray.700"
                                p={2}
                                rounded="md"
                                display="flex"
                                justifyContent="space-between"
                                alignItems="center"
                            >
                                <Text>{r.userUsername}</Text>
                                <HStack>
                                    <Button
                                        size="sm"
                                        colorScheme="green"
                                        onClick={() => handleAccept(r.userUsername)}
                                    >
                                        Accept
                                    </Button>
                                    <Button
                                        size="sm"
                                        colorScheme="red"
                                        onClick={() => handleReject(r.userUsername)}
                                    >
                                        Reject
                                    </Button>
                                </HStack>
                            </Box>
                        ))
                    ) : (
                        <Text fontSize="sm" color="gray.400">
                            No pending requests
                        </Text>
                    )}
                </VStack>
            </Box>
        </Flex>
    );
}
