import { useState, useRef, useEffect } from 'react';
import { MessageSquare, X, Send, Bot, User, Minimize2, Maximize2 } from 'lucide-react';
import { api } from '../services/api';

interface Message {
    id: number;
    text: string;
    sender: 'user' | 'bot';
    timestamp: Date;
}

export default function AiChatbot() {
    const [isOpen, setIsOpen] = useState(false);
    const [isMinimized, setIsMinimized] = useState(false);
    const [inputText, setInputText] = useState('');
    const [messages, setMessages] = useState<Message[]>([
        {
            id: 1,
            text: "Hello! I'm your Market Price Assistant. I can help you find the best prices, compare markets, or suggest products. How can I help you today?",
            sender: 'bot',
            timestamp: new Date()
        }
    ]);
    const messagesEndRef = useRef<HTMLDivElement>(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages, isOpen]);

    const handleSendMessage = async (e?: React.FormEvent) => {
        e?.preventDefault();
        if (!inputText.trim()) return;

        const newUserMessage: Message = {
            id: Date.now(),
            text: inputText,
            sender: 'user',
            timestamp: new Date()
        };

        setMessages(prev => [...prev, newUserMessage]);
        setInputText('');

        try {
            const response = await api.sendMessage(newUserMessage.text);

            const botResponse: Message = {
                id: Date.now() + 1,
                text: response.reply || "I'm sorry, I couldn't understand that.",
                sender: 'bot',
                timestamp: new Date()
            };
            setMessages(prev => [...prev, botResponse]);
        } catch (error) {
            const errorResponse: Message = {
                id: Date.now() + 1,
                text: "Sorry, I'm having trouble connecting to the server. Please check your API key configuration.",
                sender: 'bot',
                timestamp: new Date()
            };
            setMessages(prev => [...prev, errorResponse]);
        }
    };

    if (!isOpen) {
        return (
            <button
                onClick={() => setIsOpen(true)}
                className="fixed bottom-6 right-6 bg-gradient-to-r from-green-600 to-emerald-600 text-white p-4 rounded-full shadow-lg hover:shadow-xl hover:scale-110 transition-all duration-300 z-50 group"
            >
                <MessageSquare size={28} className="group-hover:animate-bounce" />
                <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold px-2 py-0.5 rounded-full border-2 border-white">
                    1
                </span>
            </button>
        );
    }

    return (
        <div
            className={`fixed bottom-6 right-6 bg-white rounded-2xl shadow-2xl z-50 transition-all duration-300 flex flex-col overflow-hidden border border-gray-100 ${isMinimized ? 'w-72 h-14' : 'w-80 sm:w-96 h-[500px] max-h-[80vh]'
                }`}
        >
            {/* Header */}
            <div
                className="bg-gradient-to-r from-green-600 to-emerald-600 p-4 flex items-center justify-between cursor-pointer"
                onClick={() => setIsMinimized(!isMinimized)}
            >
                <div className="flex items-center gap-3">
                    <div className="bg-white/20 p-1.5 rounded-lg backdrop-blur-sm">
                        <Bot size={20} className="text-white" />
                    </div>
                    <div>
                        <h3 className="font-bold text-white text-sm">Market Assistant</h3>
                        {!isMinimized && <p className="text-green-100 text-xs flex items-center gap-1"><span className="w-1.5 h-1.5 bg-green-300 rounded-full animate-pulse"></span> Online</p>}
                    </div>
                </div>
                <div className="flex items-center gap-2">
                    <button
                        onClick={(e) => { e.stopPropagation(); setIsMinimized(!isMinimized); }}
                        className="text-white/80 hover:text-white p-1 hover:bg-white/10 rounded transition-colors"
                    >
                        {isMinimized ? <Maximize2 size={16} /> : <Minimize2 size={16} />}
                    </button>
                    <button
                        onClick={(e) => { e.stopPropagation(); setIsOpen(false); }}
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
                                    className={`max-w-[75%] p-3 rounded-2xl text-sm ${msg.sender === 'user'
                                        ? 'bg-green-600 text-white rounded-tr-none'
                                        : 'bg-white text-gray-800 shadow-sm border border-gray-100 rounded-tl-none'
                                        }`}
                                >
                                    <p>{msg.text}</p>
                                    <span className={`text-[10px] mt-1 block opacity-70 ${msg.sender === 'user' ? 'text-green-100' : 'text-gray-400'
                                        }`}>
                                        {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                    </span>
                                </div>
                            </div>
                        ))}
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
                                className="flex-1 bg-transparent border-none focus:ring-0 text-sm placeholder-gray-400"
                            />
                            <button
                                type="submit"
                                disabled={!inputText.trim()}
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