import {
    Box,
    Button,
    Heading,
    Textarea,
    VStack,
    Text,
    HStack,
    Input,
} from "@chakra-ui/react";
import { useState, useEffect } from "react";
import { API_BASE as API } from "../api";

//const API = "http://localhost:5064/api";

export default function Feed() {
    const [posts, setPosts] = useState([]);
    const [newPost, setNewPost] = useState("");
    const [newCommentByPost, setNewCommentByPost] = useState({});
    const currentUser = localStorage.getItem("username");
    const token = localStorage.getItem("token");

    // зареждане на фийда
    useEffect(() => {
        const fetchFeed = async () => {
            try {
                const res = await fetch(`${API}/posts/feed`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!res.ok) throw new Error(await res.text());
                const data = await res.json();
                setPosts(data.items || []);
            } catch (err) {
                console.error("Load feed error:", err);
            }
        };
        fetchFeed();
    }, [token]);

    // създаване на пост
    const handlePost = async () => {
        if (!newPost.trim()) return;
        try {
            const res = await fetch(`${API}/posts/create`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ content: newPost }),
            });
            if (!res.ok) throw new Error(await res.text());
            const created = await res.json();
            setPosts((prev) => [created, ...prev]);
            setNewPost("");
        } catch (err) {
            console.error("Create post error:", err);
        }
    };

    // like/unlike
    const handleLike = async (postId) => {
        try {
            const res = await fetch(`${API}/posts/like/${postId}`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            const updated = await res.json();
            setPosts((prev) => prev.map((p) => (p.id === postId ? updated : p)));
        } catch (err) {
            console.error("Like error:", err);
        }
    };

    // delete пост
    const handleDeletePost = async (postId) => {
        try {
            const res = await fetch(`${API}/posts/${postId}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            setPosts((prev) => prev.filter((p) => p.id !== postId));
        } catch (err) {
            console.error("Delete post error:", err);
        }
    };

    // добавяне на коментар
    const handleAddComment = async (postId) => {
        const content = (newCommentByPost[postId] || "").trim();
        if (!content) return;
        try {
            const res = await fetch(`${API}/comments/post/${postId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ content }),
            });
            if (!res.ok) throw new Error(await res.text());
            const created = await res.json();
            setPosts((prev) =>
                prev.map((p) =>
                    p.id === postId ? { ...p, comments: [created, ...p.comments] } : p
                )
            );
            setNewCommentByPost((s) => ({ ...s, [postId]: "" }));
        } catch (err) {
            console.error("Add comment error:", err);
        }
    };

    // триене на коментар
    const handleDeleteComment = async (postId, commentId) => {
        try {
            const res = await fetch(`${API}/comments/${commentId}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            setPosts((prev) =>
                prev.map((p) =>
                    p.id === postId
                        ? {
                            ...p,
                            comments: p.comments.filter((c) => c.id !== commentId),
                        }
                        : p
                )
            );
        } catch (err) {
            console.error("Delete comment error:", err);
        }
    };

    return (
        <Box bg="gray.900" color="white" minH="100vh" w="100vw" pt="80px" px={6}>
            <Heading mb={6}>Feed</Heading>

            {/* форма за нов пост */}
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

            {/* постове */}
            <VStack spacing={6} align="stretch">
                {posts.map((post) => (
                    <Box key={post.id} bg="gray.800" p={4} rounded="md" shadow="md">
                        <Text fontWeight="bold">{post.author}</Text>
                        <Text mb={3}>{post.content}</Text>

                        {/* броячи */}
                        <HStack spacing={6} mb={3}>
                            <Text fontSize="sm" color="gray.400">
                                ❤️ {post.likes} {post.likes === 1 ? "like" : "likes"}
                            </Text>
                            <Text fontSize="sm" color="gray.400">
                                💬 {post.comments?.length || 0}{" "}
                                {post.comments?.length === 1 ? "comment" : "comments"}
                            </Text>
                        </HStack>

                        {/* бутони */}
                        <HStack spacing={3} mb={3}>
                            <Button size="sm" onClick={() => handleLike(post.id)}>
                                {post.likedByCurrentUser ? "💔 Unlike" : "❤️ Like"}
                            </Button>
                            {post.author === currentUser && (
                                <Button
                                    size="sm"
                                    colorScheme="red"
                                    onClick={() => handleDeletePost(post.id)}
                                >
                                    Delete Post
                                </Button>
                            )}
                        </HStack>

                        {/* коментари */}
                        <VStack align="stretch" spacing={2} mt={3}>
                            <HStack>
                                <Input
                                    size="sm"
                                    bg="gray.700"
                                    border="none"
                                    placeholder="Write a comment..."
                                    value={newCommentByPost[post.id] || ""}
                                    onChange={(e) =>
                                        setNewCommentByPost((s) => ({
                                            ...s,
                                            [post.id]: e.target.value,
                                        }))
                                    }
                                />
                                <Button
                                    size="sm"
                                    colorScheme="blue"
                                    onClick={() => handleAddComment(post.id)}
                                >
                                    Comment
                                </Button>
                            </HStack>

                            {post.comments && post.comments.length > 0 ? (
                                post.comments.map((c) => (
                                    <Box key={c.id} bg="gray.700" p={2} rounded="md">
                                        <HStack justify="space-between">
                                            <Text fontWeight="bold">{c.author}</Text>
                                            {c.author === currentUser && (
                                                <Button
                                                    size="xs"
                                                    colorScheme="red"
                                                    variant="outline"
                                                    onClick={() => handleDeleteComment(post.id, c.id)}
                                                >
                                                    Delete
                                                </Button>
                                            )}
                                        </HStack>
                                        <Text>{c.content}</Text>
                                    </Box>
                                ))
                            ) : (
                                <Text fontSize="sm" color="gray.400">
                                    No comments yet
                                </Text>
                            )}
                        </VStack>
                    </Box>
                ))}
            </VStack>
        </Box>
    );
}
