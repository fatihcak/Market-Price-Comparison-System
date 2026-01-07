import { useState, useRef, useEffect } from 'react';
import { X, Send, Bot, User, /*Minimize2, Maximize2*/ } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import { api } from '../services/api';


interface Message {
    id: number;
    text: string;
    sender: 'user' | 'bot';
    timestamp: Date;
}

interface AiChatbotProps {
    hideOnMobile?: boolean;
}

export default function AiChatbot({ hideOnMobile = false }: AiChatbotProps) {
    const [/*isOpen/*, /*setIsOpen*/] = useState(true);
    const [isMinimized, setIsMinimized] = useState(true);
    const [inputText, setInputText] = useState('');
    const [isLoading, setIsLoading] = useState(false);
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

    const handleSendMessage = async (e?: React.FormEvent) => {
        e?.preventDefault();
        if (!inputText.trim() || isLoading) return; // Prevent double submission

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
                // onComplete
                () => {
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


    return (
        <div
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
                        onClick={(e) => { e.stopPropagation(); setIsMinimized(true); }}
                        className="text-white/80 hover:text-white p-1 hover:bg-white/10 rounded transition-colors"
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

                        <div ref={messagesEndRef} />
                    </div>

                    {/* Input Area */}
                    <form onSubmit={handleSendMessage} className="p-3 bg-white border-t border-gray-100">
                        <div className="flex items-center gap-2 bg-gray-50 rounded-full px-4 py-2 border border-gray-200 focus-within:border-green-500 focus-within:ring-1 focus-within:ring-green-500 transition-all">
                            <input
                                type="text"
                                value={inputText}
                                onChange={(e) => setInputText(e.target.value)}
                                placeholder="Ask about prices..."
                                className="flex-1 bg-transparent border-none focus:ring-0 outline-none text-sm placeholder-gray-400"
                            />
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
        </div>
    );
}