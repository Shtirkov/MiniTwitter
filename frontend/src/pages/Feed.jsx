import {
    Box,
    Button,
    Heading,
    Textarea,
    VStack,
    Text,
    HStack,
    Input,
    Divider,
    Spinner,
} from "@chakra-ui/react";
import { useState, useEffect } from "react";

const API = "http://localhost:5064/api";

export default function Feed() {
    const [posts, setPosts] = useState([]);
    const [newPost, setNewPost] = useState("");

    // comments state по пост: { [postId]: { loading, items: Comment[] } }
    const [commentsByPost, setCommentsByPost] = useState({});
    // локален input за нов коментар по пост
    const [newCommentByPost, setNewCommentByPost] = useState({});
    // отворени/затворени секции с коментари
    const [expanded, setExpanded] = useState({});
    const currentUser = localStorage.getItem("username");
    const token = localStorage.getItem("token");

    // load feed
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

    // create post
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

    // toggle like
    const handleLike = async (postId) => {
        try {
            // оптимистично +1/−1 ако имаме likedByCurrentUser
            setPosts((prev) =>
                prev.map((p) =>
                    p.id === postId
                        ? {
                            ...p,
                            likesCount:
                                p.likesCount + (p.likedByCurrentUser ? -1 : 1),
                            likedByCurrentUser: !p.likedByCurrentUser,
                        }
                        : p
                )
            );

            const res = await fetch(`${API}/posts/like/${postId}`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());
            const updated = await res.json(); // връща поста с точните стойности
            setPosts((prev) => prev.map((p) => (p.id === postId ? updated : p)));
        } catch (err) {
            console.error("Like error:", err);
            // по желание: рефреш на фийда или revert на оптимистичното обновяване
        }
    };

    // delete post
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

    // show/hide comments + lazy load
    const toggleComments = async (postId) => {
        setExpanded((e) => ({ ...e, [postId]: !e[postId] }));

        // ако още не сме зареждали коментарите — взимаме ги
        if (!commentsByPost[postId]) {
            setCommentsByPost((m) => ({ ...m, [postId]: { loading: true, items: [] } }));
            try {
                const res = await fetch(`${API}/comments/post/${postId}`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!res.ok) throw new Error(await res.text());
                const items = await res.json();
                setCommentsByPost((m) => ({ ...m, [postId]: { loading: false, items } }));
            } catch (err) {
                console.error("Load comments error:", err);
                setCommentsByPost((m) => ({ ...m, [postId]: { loading: false, items: [] } }));
            }
        }
    };

    // add comment
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

            // добавяме коментара локално
            setCommentsByPost((m) => ({
                ...m,
                [postId]: {
                    loading: false,
                    items: [created, ...(m[postId]?.items || [])],
                },
            }));
            // увеличаваме локално броя коментари, ако имаш поле commentsCount
            setPosts((prev) =>
                prev.map((p) =>
                    p.id === postId
                        ? { ...p, commentsCount: (p.commentsCount ?? 0) + 1 }
                        : p
                )
            );
            // чистим input-а
            setNewCommentByPost((s) => ({ ...s, [postId]: "" }));
        } catch (err) {
            console.error("Add comment error:", err);
        }
    };

    // delete comment
    const handleDeleteComment = async (postId, commentId) => {
        try {
            const res = await fetch(`${API}/comments/${commentId}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error(await res.text());

            setCommentsByPost((m) => ({
                ...m,
                [postId]: {
                    loading: false,
                    items: (m[postId]?.items || []).filter((c) => c.id !== commentId),
                },
            }));
            setPosts((prev) =>
                prev.map((p) =>
                    p.id === postId
                        ? { ...p, commentsCount: Math.max((p.commentsCount ?? 1) - 1, 0) }
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

            {/* нов пост */}
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
                {posts.map((post) => {
                    const commentsState = commentsByPost[post.id];
                    const comments = commentsState?.items || [];
                    const commentsLoading = commentsState?.loading;
                    const commentsCount =
                        post.commentsCount ?? comments.length ?? 0;

                    return (
                        <Box key={post.id} bg="gray.800" p={4} rounded="md" shadow="md">
                            <Text fontWeight="bold">{post.author}</Text>
                            <Text mb={3}>{post.content}</Text>

                            {/* likes & comments counters */}
                            <HStack spacing={6} mb={3}>
                                <Text fontSize="sm" color="gray.400">
                                    ❤️ {post.likesCount} {post.likesCount === 1 ? "like" : "likes"}
                                </Text>
                                <Text fontSize="sm" color="gray.400">
                                    💬 {commentsCount} {commentsCount === 1 ? "comment" : "comments"}
                                </Text>
                            </HStack>

                            <HStack spacing={3} mb={3} wrap="wrap">
                                <Button size="sm" onClick={() => handleLike(post.id)}>
                                    {post.likedByCurrentUser ? "💔 Unlike" : "❤️ Like"}
                                </Button>

                                <Button
                                    size="sm"
                                    variant="outline"
                                    color="gray.400"
                                    onClick={() => toggleComments(post.id)}
                                >
                                    {expanded[post.id] ? "Hide comments" : "Show comments"}
                                </Button>

                                {post.author === currentUser && (
                                    <Button
                                        size="sm"
                                        colorScheme="red"
                                        onClick={() => handleDeletePost(post.id)}
                                    >
                                        Delete
                                    </Button>
                                )}
                            </HStack>

                            {/* коментари (секция) */}
                            {expanded[post.id] && (
                                <Box bg="gray.700" p={3} rounded="md">
                                    {/* input + add */}
                                    <HStack mb={3}>
                                        <Input
                                            size="sm"
                                            bg="gray.600"
                                            border="none"
                                            value={newCommentByPost[post.id] || ""}
                                            onChange={(e) =>
                                                setNewCommentByPost((s) => ({
                                                    ...s,
                                                    [post.id]: e.target.value,
                                                }))
                                            }
                                            placeholder="Write a comment..."
                                            _placeholder={{ color: "gray.300" }}
                                        />
                                        <Button
                                            size="sm"
                                            colorScheme="blue"
                                            onClick={() => handleAddComment(post.id)}
                                        >
                                            Comment
                                        </Button>
                                    </HStack>

                                    <Divider mb={3} borderColor="gray.600" />

                                    {commentsLoading ? (
                                        <HStack>
                                            <Spinner size="sm" />
                                            <Text>Loading comments…</Text>
                                        </HStack>
                                    ) : comments.length === 0 ? (
                                        <Text color="gray.300">No comments yet</Text>
                                    ) : (
                                        <VStack spacing={2} align="stretch">
                                            {comments.map((c) => (
                                                <Box key={c.id} bg="gray.800" p={2} rounded="md">
                                                    <HStack justify="space-between">
                                                        <Text fontWeight="bold">{c.author}</Text>
                                                        {c.author === currentUser && (
                                                            <Button
                                                                size="xs"
                                                                colorScheme="red"
                                                                variant="outline"
                                                                onClick={() =>
                                                                    handleDeleteComment(post.id, c.id)
                                                                }
                                                            >
                                                                Delete
                                                            </Button>
                                                        )}
                                                    </HStack>
                                                    <Text>{c.content}</Text>
                                                </Box>
                                            ))}
                                        </VStack>
                                    )}
                                </Box>
                            )}
                        </Box>
                    );
                })}
            </VStack>
        </Box>
    );
}
