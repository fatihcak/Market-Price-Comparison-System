import { useState, useRef, useEffect } from 'react';
import { X, Send, Bot, User, ShoppingCart, TrendingDown, Trash2, Plus, Mic, MicOff, RotateCcw, Eye, Heart } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import { api } from '../services/api';
import { Product } from '../types';

// Product returned from chatbot RAG
interface ChatProduct {
    id: number;
    name: string;
    price: number;
    market: string;
    category?: string;
    imageUrl?: string;
}

interface Message {
    id: number;
    text: string;
    sender: 'user' | 'bot';
    timestamp: Date;
    products?: ChatProduct[];
}

interface AiChatbotProps {
    hideOnMobile?: boolean;
    onAddToCart?: (product: Product) => void;
    onOpenList?: () => void;
    onCompareList?: () => void;
    onClearList?: () => void;
    onOpenFavorites?: () => void;
}

export default function AiChatbot({ hideOnMobile = false, onAddToCart, onOpenList, onCompareList, onClearList, onOpenFavorites }: AiChatbotProps) {
    const [isHidden, setIsHidden] = useState(false);
    const [isMinimized, setIsMinimized] = useState(true);
    const [inputText, setInputText] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState<ChatProduct | null>(null);
    const [messages, setMessages] = useState<Message[]>(() => {
        const saved = sessionStorage.getItem('chat_history');
        if (saved) {
            try {
                const parsedMessages = JSON.parse(saved);
                return parsedMessages.map((msg: any) => ({
                    ...msg,
                    timestamp: new Date(msg.timestamp)
                }));
            } catch (error) {
                console.error('Error parsing chat history:', error);
            }
        }
        return [{
            id: 1,
            text: "Hello! I'm your Market Price Assistant. I can help you find the best prices, compare markets, or suggest products. How can I help you today?",
            sender: 'bot',
            timestamp: new Date()
        }];
    });
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const chatbotRef = useRef<HTMLDivElement>(null);

    // Speech recognition
    const [isListening, setIsListening] = useState(false);
    const [isVoiceSupported, setIsVoiceSupported] = useState(true);
    const recognitionRef = useRef<any>(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
        // If the chat is opening (not minimized), ensuring we scroll to bottom after animation
        if (!isMinimized) {
            const timer = setTimeout(scrollToBottom, 310);
            return () => clearTimeout(timer);
        }
    }, [messages, isMinimized, isLoading]);

    // Save messages to LocalStorage whenever the 'messages' array changes
    useEffect(() => {
        sessionStorage.setItem('chat_history', JSON.stringify(messages));
    }, [messages]);

    // Click outside to close
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (chatbotRef.current && !chatbotRef.current.contains(event.target as Node) && !isMinimized) {
                setIsMinimized(true);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [isMinimized]);

    // Speech recognition setup
    useEffect(() => {
        const SpeechRecognition = (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition;
        if (!SpeechRecognition) {
            setIsVoiceSupported(false);
            return;
        }

        const recognition = new SpeechRecognition();
        recognition.continuous = false;
        recognition.interimResults = true;
        recognition.lang = 'en-US'; // English for chatbot

        recognition.onstart = () => {
            setIsListening(true);
        };

        recognition.onend = () => {
            setIsListening(false);
        };

        recognition.onerror = (event: any) => {
            console.error('Speech recognition error:', event.error);
            setIsListening(false);
        };

        recognition.onresult = (event: any) => {
            const transcript = Array.from(event.results)
                .map((result: any) => result[0].transcript)
                .join('');

            setInputText(transcript);

            // If this is a final result, send the message
            if (event.results[0].isFinal) {
                // Small delay to let user see what was transcribed
                setTimeout(() => {
                    const form = document.getElementById('chatbot-form') as HTMLFormElement;
                    if (form) form.requestSubmit();
                }, 300);
            }
        };

        recognitionRef.current = recognition;

        return () => {
            if (recognitionRef.current) {
                recognitionRef.current.abort();
            }
        };
    }, []);

    const toggleVoiceInput = () => {
        if (!isVoiceSupported || !recognitionRef.current) {
            alert('Voice input is not supported in your browser. Please use Chrome or Edge.');
            return;
        }

        if (isListening) {
            recognitionRef.current.stop();
        } else {
            try {
                recognitionRef.current.start();
            } catch (error) {
                console.error('Error starting speech recognition:', error);
            }
        }
    };

    const handleSendMessage = async (e?: React.FormEvent) => {
        e?.preventDefault();
        if (!inputText.trim() || isLoading) return; // Prevent double submission

        const lowerInput = inputText.toLowerCase().trim();

        // Check for frontend-only commands
        if (lowerInput.includes('calculate') || lowerInput.includes('best price') || lowerInput.includes('compare my')) {
            if (onCompareList) {
                onCompareList();
                setInputText('');
                return;
            }
        }

        if (lowerInput.includes('show my list') || lowerInput.includes('my shopping list') || lowerInput === 'my list') {
            if (onOpenList) {
                onOpenList();
                setInputText('');
                return;
            }
        }

        if (lowerInput.includes('clear my list') || lowerInput.includes('clear list') || lowerInput.includes('empty my list')) {
            if (onClearList) {
                onClearList();
                setInputText('');
                return;
            }
        }

        const userMessageId = Date.now();
        const botMessageId = userMessageId + 1;

        const newUserMessage: Message = {
            id: userMessageId,
            text: inputText,
            sender: 'user',
            timestamp: new Date()
        };

        // Only add user message first
        setMessages(prev => [...prev, newUserMessage]);
        setInputText('');
        setIsLoading(true);

        // Generate or use existing sessionId
        const sessionId = sessionStorage.getItem('chat_session_id') || `session-${Date.now()}`;
        sessionStorage.setItem('chat_session_id', sessionId);

        let botMessageAdded = false;

        try {
            await api.sendMessageStream(
                newUserMessage.text,
                sessionId,
                // onChunk: append to bot message
                (chunk: string) => {
                    if (!botMessageAdded) {
                        // First chunk - add the bot message
                        botMessageAdded = true;
                        setMessages(prev => [...prev, {
                            id: botMessageId,
                            text: chunk,
                            sender: 'bot',
                            timestamp: new Date()
                        }]);
                    } else {
                        // Subsequent chunks - append to existing message
                        setMessages(prev =>
                            prev.map(msg =>
                                msg.id === botMessageId
                                    ? { ...msg, text: msg.text + chunk }
                                    : msg
                            )
                        );
                    }
                },
                // onComplete: fetch products after streaming
                async () => {
                    // After streaming completes, fetch products from non-streaming endpoint
                    // Use a different sessionId to avoid history pollution
                    try {
                        const productFetchSessionId = `product-fetch-${Date.now()}`;
                        const productResponse = await api.sendMessage(newUserMessage.text, productFetchSessionId);
                        console.log('Product response:', productResponse); // DEBUG
                        if (productResponse.foundProducts && productResponse.foundProducts.length > 0) {
                            console.log('Found products:', productResponse.foundProducts); // DEBUG

                            // If this was an 'add' command, automatically add products to the frontend cart
                            const lowerText = newUserMessage.text.toLowerCase();
                            if (lowerText.includes('add') && onAddToCart) {
                                productResponse.foundProducts.forEach((product: any) => {
                                    onAddToCart({
                                        id: product.id,
                                        name: product.name,
                                        productName: product.name,
                                        price: product.price,
                                        oldPrice: null,
                                        market: product.market,
                                        discount: 0,
                                        category: product.category || 'Other',
                                        image: product.imageUrl || '/placeholder.png'
                                    });
                                });
                            }

                            setMessages(prev =>
                                prev.map(msg =>
                                    msg.id === botMessageId
                                        ? { ...msg, products: productResponse.foundProducts }
                                        : msg
                                )
                            );
                        }
                    } catch (e) {
                        console.warn('Could not fetch products:', e);
                    }
                    setIsLoading(false);
                },
                // onError
                () => {
                    if (!botMessageAdded) {
                        setMessages(prev => [...prev, {
                            id: botMessageId,
                            text: "Sorry, I'm having trouble connecting to the server. Please try again.",
                            sender: 'bot',
                            timestamp: new Date()
                        }]);
                    } else {
                        setMessages(prev =>
                            prev.map(msg =>
                                msg.id === botMessageId
                                    ? { ...msg, text: msg.text || "Sorry, I'm having trouble connecting to the server." }
                                    : msg
                            )
                        );
                    }
                    setIsLoading(false);
                }
            );
        } catch {
            setMessages(prev => [...prev, {
                id: botMessageId,
                text: "Sorry, I'm having trouble connecting to the server. Please check your connection.",
                sender: 'bot',
                timestamp: new Date()
            }]);
            setIsLoading(false);
        }
    };

    // Quick action handler
    const sendQuickAction = async (message: string) => {
        if (isLoading) return;

        const userMessageId = Date.now();
        const botMessageId = userMessageId + 1;

        const newUserMessage: Message = {
            id: userMessageId,
            text: message,
            sender: 'user',
            timestamp: new Date()
        };

        setMessages(prev => [...prev, newUserMessage]);
        setIsLoading(true);

        const sessionId = sessionStorage.getItem('chat_session_id') || `session-${Date.now()}`;
        sessionStorage.setItem('chat_session_id', sessionId);

        let botMessageAdded = false;

        try {
            await api.sendMessageStream(
                message,
                sessionId,
                (chunk: string) => {
                    if (!botMessageAdded) {
                        botMessageAdded = true;
                        setMessages(prev => [...prev, {
                            id: botMessageId,
                            text: chunk,
                            sender: 'bot',
                            timestamp: new Date()
                        }]);
                    } else {
                        setMessages(prev =>
                            prev.map(msg =>
                                msg.id === botMessageId
                                    ? { ...msg, text: msg.text + chunk }
                                    : msg
                            )
                        );
                    }
                },
                // onComplete: fetch products after streaming
                async () => {
                    try {
                        const productFetchSessionId = `product-fetch-${Date.now()}`;
                        const productResponse = await api.sendMessage(message, productFetchSessionId);
                        if (productResponse.foundProducts && productResponse.foundProducts.length > 0) {
                            setMessages(prev =>
                                prev.map(msg =>
                                    msg.id === botMessageId
                                        ? { ...msg, products: productResponse.foundProducts }
                                        : msg
                                )
                            );
                        }
                    } catch (e) {
                        console.warn('Could not fetch products:', e);
                    }
                    setIsLoading(false);
                },
                () => {
                    if (!botMessageAdded) {
                        setMessages(prev => [...prev, {
                            id: botMessageId,
                            text: "Connection error.",
                            sender: 'bot',
                            timestamp: new Date()
                        }]);
                    }
                    setIsLoading(false);
                }
            );
        } catch {
            setMessages(prev => [...prev, {
                id: botMessageId,
                text: "Sunucuya bağlanılamadı.",
                sender: 'bot',
                timestamp: new Date()
            }]);
            setIsLoading(false);
        }
    };

    // Clear chat history
    const clearChatHistory = () => {
        if (window.confirm('Clear chat history? This will start a new conversation.')) {
            const welcomeMessage: Message = {
                id: Date.now(),
                text: "Hello! I'm your Market Price Assistant. I can help you find the best prices, compare markets, or suggest products. How can I help you today?",
                sender: 'bot',
                timestamp: new Date()
            };
            setMessages([welcomeMessage]);
            sessionStorage.removeItem('chat_history');
            sessionStorage.removeItem('chat_session_id');
        }
    };

    // Quick actions configuration - actions with callbacks don't send chat messages
    const quickActions = [
        { icon: ShoppingCart, label: 'My List', color: 'bg-blue-500 hover:bg-blue-600', action: onOpenList },
        { icon: Heart, label: 'Favorites', color: 'bg-red-500 hover:bg-red-600', action: onOpenFavorites },
        { icon: TrendingDown, label: 'Best Price', color: 'bg-green-500 hover:bg-green-600', action: onCompareList },
        { icon: Trash2, label: 'Clear', color: 'bg-orange-500 hover:bg-orange-600', action: onClearList },
    ];

    // Suggestion chips - shown when conversation is new
    const suggestionChips = [
        "What's the price of milk?",
        "Add eggs to my list",
        "Compare tomato prices",
        "Find cheapest bread",
    ];

    const showSuggestions = messages.length <= 2;



    // If hidden, show only floating button
    if (isHidden) {
        return (
            <button
                onClick={() => { setIsHidden(false); setIsMinimized(false); }}
                className={`fixed bottom-3 left-2 bg-green-600 hover:bg-green-700 text-white p-4 rounded-full shadow-2xl z-40 transition-all duration-300 hover:scale-110 ${hideOnMobile ? 'hidden md:flex' : 'flex'} items-center justify-center`}
                title="Open Market Assistant"
            >
                <Bot size={24} className="text-white" />
            </button>
        );
    }

    return (
        <div
            ref={chatbotRef}
            className={`fixed bottom-3 left-2 bg-white rounded-2xl shadow-2xl z-40 transition-all duration-300 ${hideOnMobile ? 'hidden md:flex' : 'flex'} flex-col overflow-hidden ${isMinimized ? 'w-72 h-14' : 'w-80 sm:w-96 h-[500px] max-h-[80vh]'}`}
        >
            {/* Header */}
            <div
                className="bg-green-600 p-4 flex items-center justify-between cursor-pointer"
                onClick={() => setIsMinimized(!isMinimized)}
            >
                <div className="flex items-center gap-3">
                    <div className="bg-white/20 p-0.5 rounded-lg backdrop-blur-sm">
                        <Bot size={20} className="text-white" />
                    </div>
                    <div>
                        <h3 className="font-bold text-white text-sm">Market Assistant</h3>
                        {!isMinimized && <p className="text-green-100 text-xs flex items-center gap-1"><span className="w-1.5 h-1.5 bg-green-300 rounded-full animate-pulse"></span> Online</p>}
                    </div>
                </div>
                <div className="flex items-center gap-2">
                    <button
                        onClick={(e) => { e.stopPropagation(); clearChatHistory(); }}
                        className="text-white/80 hover:text-white p-1 hover:bg-white/10 rounded transition-colors"
                        title="Clear chat history"
                    >
                        <RotateCcw size={16} />
                    </button>
                    <button
                        onClick={(e) => { e.stopPropagation(); setIsHidden(true); }}
                        className="text-white/80 hover:text-white p-1 hover:bg-white/10 rounded transition-colors"
                        title="Hide chatbot"
                    >
                        <X size={18} />
                    </button>
                </div>
            </div>

            {/* Chat Area */}
            {!isMinimized && (
                <>
                    <div className="flex-1 overflow-y-auto p-4 bg-gray-50 space-y-4">
                        {messages.map((msg) => (
                            <div
                                key={msg.id}
                                className={`flex items-start gap-2.5 ${msg.sender === 'user' ? 'flex-row-reverse' : ''}`}
                            >
                                <div className={`w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0 ${msg.sender === 'user' ? 'bg-gray-200' : 'bg-green-100'
                                    }`}>
                                    {msg.sender === 'user' ? (
                                        <User size={16} className="text-gray-600" />
                                    ) : (
                                        <Bot size={16} className="text-green-600" />
                                    )}
                                </div>
                                <div
                                    className={`max-w-[85%] p-3.5 rounded-2xl text-sm leading-relaxed ${msg.sender === 'user'
                                        ? 'bg-green-600 text-white rounded-tr-none shadow-md'
                                        : 'bg-white text-gray-800 shadow-md border border-gray-100 rounded-tl-none'
                                        }`}
                                >
                                    <div className={`prose prose-sm max-w-none ${msg.sender === 'user' ? 'prose-invert' : ''}`}>
                                        <ReactMarkdown>{msg.text}</ReactMarkdown>
                                    </div>
                                    <span className={`text-[10px] mt-2 block font-medium opacity-60 ${msg.sender === 'user' ? 'text-green-50' : 'text-gray-400'
                                        }`}>
                                        {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                    </span>

                                    {/* Product Cards from RAG */}
                                    {msg.products && msg.products.length > 0 && onAddToCart && (
                                        <div className="mt-3 space-y-2 border-t border-gray-100 pt-3">
                                            <p className="text-xs text-gray-500 font-medium">Found Products:</p>
                                            {msg.products.map((product) => (
                                                <div
                                                    key={product.id}
                                                    className="flex items-center gap-2 bg-gray-50 rounded-lg p-2 hover:bg-gray-100 transition-colors cursor-pointer"
                                                    onClick={() => setSelectedProduct(product)}
                                                >
                                                    {/* Product Image */}
                                                    <div className="w-10 h-10 rounded-lg overflow-hidden bg-gray-200 flex-shrink-0">
                                                        {product.imageUrl ? (
                                                            <img
                                                                src={product.imageUrl}
                                                                alt={product.name}
                                                                className="w-full h-full object-cover"
                                                                onError={(e) => {
                                                                    (e.target as HTMLImageElement).src = '/placeholder.png';
                                                                }}
                                                            />
                                                        ) : (
                                                            <div className="w-full h-full flex items-center justify-center text-gray-400">
                                                                <ShoppingCart size={16} />
                                                            </div>
                                                        )}
                                                    </div>
                                                    {/* Product Info */}
                                                    <div className="flex-1 min-w-0">
                                                        <p className="text-xs font-medium text-gray-800 truncate">{product.name}</p>
                                                        <p className="text-[10px] text-gray-500">{product.market} - {product.price.toFixed(2)} TL</p>
                                                    </div>
                                                    <button
                                                        onClick={(e) => {
                                                            e.stopPropagation();
                                                            onAddToCart({
                                                                id: product.id,
                                                                name: product.name,
                                                                productName: product.name,
                                                                price: product.price,
                                                                oldPrice: null,
                                                                market: product.market,
                                                                discount: 0,
                                                                category: product.category || 'Other',
                                                                image: product.imageUrl || '/placeholder.png'
                                                            });
                                                        }}
                                                        className="ml-2 p-1.5 bg-green-500 hover:bg-green-600 text-white rounded-full transition-colors flex-shrink-0"
                                                        title="Add to cart"
                                                    >
                                                        <Plus size={12} />
                                                    </button>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            </div>
                        ))}

                        {isLoading && (
                            <div className="flex items-start gap-2.5">
                                <div className="w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0 bg-green-100">
                                    <Bot size={16} className="text-green-600" />
                                </div>
                                <div className="bg-white text-gray-800 shadow-sm border border-gray-100 rounded-2xl rounded-tl-none p-4">
                                    <div className="flex gap-1.5 items-center h-4">
                                        <div className="w-1 h-1 bg-gray-900 rounded-full animate-bounce animate-pulse [animation-delay:-0.3s]"></div>
                                        <div className="w-1 h-1 bg-gray-900 rounded-full animate-bounce animate-pulse [animation-delay:-0.15s]"></div>
                                        <div className="w-1 h-1 bg-gray-900 rounded-full animate-bounce animate-pulse"></div>
                                    </div>
                                </div>
                            </div>
                        )}

                        {/* Suggestion Chips - shown at conversation start */}
                        {showSuggestions && !isLoading && (
                            <div className="flex flex-wrap gap-2 mt-2">
                                {suggestionChips.map((chip, index) => (
                                    <button
                                        key={index}
                                        onClick={() => sendQuickAction(chip)}
                                        className="px-3 py-1.5 bg-white border border-green-200 text-green-700 text-xs rounded-full hover:bg-green-50 hover:border-green-400 transition-all shadow-sm"
                                    >
                                        {chip}
                                    </button>
                                ))}
                            </div>
                        )}

                        <div ref={messagesEndRef} />
                    </div>

                    {/* Quick Actions */}
                    <div className="px-3 py-2 border-t border-gray-100 bg-gray-50">
                        <div className="flex gap-2 overflow-x-auto pb-1">
                            {quickActions.map((actionItem, index) => (
                                <button
                                    key={index}
                                    onClick={() => {
                                        // Only trigger the action callback, don't send chat message
                                        if (actionItem.action) {
                                            actionItem.action();
                                        }
                                    }}
                                    disabled={isLoading}
                                    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full text-white text-xs font-medium whitespace-nowrap transition-all ${actionItem.color} disabled:opacity-50 disabled:cursor-not-allowed shadow-sm hover:shadow-md`}
                                >
                                    <actionItem.icon size={12} />
                                    {actionItem.label}
                                </button>
                            ))}
                        </div>
                    </div>

                    {/* Input Area */}
                    <form id="chatbot-form" onSubmit={handleSendMessage} className="p-3 bg-white border-t border-gray-100">
                        <div className="flex items-center gap-2 bg-gray-50 rounded-full px-4 py-2 border border-gray-200 focus-within:border-green-500 focus-within:ring-1 focus-within:ring-green-500 transition-all">
                            <input
                                type="text"
                                value={inputText}
                                onChange={(e) => setInputText(e.target.value)}
                                placeholder={isListening ? "Listening..." : "Ask about prices..."}
                                className="flex-1 bg-transparent border-none focus:ring-0 outline-none text-sm placeholder-gray-400"
                            />
                            <button
                                type="button"
                                onClick={toggleVoiceInput}
                                className={`p-1.5 rounded-full transition-all ${isListening
                                    ? 'bg-red-500 text-white animate-pulse'
                                    : isVoiceSupported
                                        ? 'text-gray-400 hover:text-green-600 hover:bg-green-50'
                                        : 'text-gray-300 cursor-not-allowed'
                                    }`}
                                disabled={!isVoiceSupported}
                                title={isVoiceSupported ? (isListening ? 'Stop listening' : 'Voice input') : 'Voice not supported'}
                            >
                                {isVoiceSupported ? <Mic size={14} /> : <MicOff size={14} />}
                            </button>
                            <button
                                type="submit"
                                disabled={!inputText.trim() || isLoading}
                                className="p-1.5 bg-green-600 text-white rounded-full hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                            >
                                <Send size={14} />
                            </button>
                        </div>
                    </form>
                </>
            )}

            {/* Product Detail Popup */}
            {selectedProduct && (
                <div
                    className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
                    onClick={() => setSelectedProduct(null)}
                >
                    <div
                        className="bg-white rounded-2xl shadow-2xl max-w-sm w-full overflow-hidden"
                        onClick={(e) => e.stopPropagation()}
                    >
                        <div className="relative h-48 bg-gradient-to-br from-green-100 to-emerald-50">
                            {selectedProduct.imageUrl ? (
                                <img
                                    src={selectedProduct.imageUrl}
                                    alt={selectedProduct.name}
                                    className="w-full h-full object-contain p-4"
                                />
                            ) : (
                                <div className="w-full h-full flex items-center justify-center">
                                    <ShoppingCart size={64} className="text-green-300" />
                                </div>
                            )}
                            <button
                                onClick={() => setSelectedProduct(null)}
                                className="absolute top-3 right-3 p-2 bg-white/80 hover:bg-white rounded-full shadow"
                            >
                                <X size={18} className="text-gray-600" />
                            </button>
                        </div>
                        <div className="p-5">
                            <h3 className="text-lg font-bold text-gray-800 mb-2">{selectedProduct.name}</h3>
                            <div className="space-y-2 mb-4">
                                <div className="flex justify-between text-sm">
                                    <span className="text-gray-500">Market:</span>
                                    <span className="font-medium">{selectedProduct.market}</span>
                                </div>
                                <div className="flex justify-between text-sm">
                                    <span className="text-gray-500">Category:</span>
                                    <span className="font-medium">{selectedProduct.category || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-500">Price:</span>
                                    <span className="text-2xl font-bold text-green-600">{selectedProduct.price.toFixed(2)} TL</span>
                                </div>
                            </div>
                            {onAddToCart && (
                                <button
                                    onClick={() => {
                                        onAddToCart({
                                            id: selectedProduct.id,
                                            name: selectedProduct.name,
                                            productName: selectedProduct.name,
                                            price: selectedProduct.price,
                                            oldPrice: null,
                                            market: selectedProduct.market,
                                            discount: 0,
                                            category: selectedProduct.category || 'Other',
                                            image: selectedProduct.imageUrl || '/placeholder.png'
                                        });
                                        setSelectedProduct(null);
                                    }}
                                    className="w-full py-3 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl flex items-center justify-center gap-2"
                                >
                                    <Plus size={18} />
                                    Add to Cart
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}