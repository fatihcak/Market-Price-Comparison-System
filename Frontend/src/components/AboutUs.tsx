import { Github, Linkedin, Mail, Code, Users } from 'lucide-react';

export default function AboutUs() {
    const teamMembers = [
        {
            name: "Doğukan Örs",
            role: "Frontend Developer",
            github: "https://github.com/DogukanOrs",
            linkedin: "https://www.linkedin.com/in/do%C4%9Fukan-bilal-%C3%B6rs-aa36882a2/",
            email: "dogukan@example.com"
        },
        {
            name: "Doğukan Güler",
            role: "Backend Developer",
            github: "https://github.com/gulerdogukan",
            linkedin: "https://www.linkedin.com/in/dogukanguler/",
            email: "dogukan@example.com"
        },
        {
            name: "Fatih Çakır",
            role: "Backend Developer",
            github: "https://github.com/fatihcak",
            linkedin: "https://www.linkedin.com/in/fatihcak/",
            email: "dogukan@example.com"
        },
        {
            name: "Aleyna Yılmaz",
            role: "Database Engineer",
            github: "https://github.com/aleynayilmz",
            linkedin: "https://www.linkedin.com/in/yilmazaleyna/",
            email: "dogukan@example.com"
        }
    ];

    return (
        <div className="max-w-4xl mx-auto px-4 py-16">
            {/* Header Section */}
            <div className="text-center mb-16">
                <h1 className="text-4xl font-bold text-gray-900 mb-4">About Us</h1>
                <p className="text-lg text-gray-600 max-w-2xl mx-auto">
                    Market Price Comparison System is a graduation project designed to help
                    consumers find the best prices across different supermarkets.
                </p>
            </div>

            {/* Project Info */}
            <div className="bg-gradient-to-r from-green-50 to-emerald-50 rounded-2xl p-8 mb-12">
                <div className="flex items-center gap-3 mb-4">
                    <Code className="text-green-600" size={28} />
                    <h2 className="text-2xl font-bold text-gray-900">The Project</h2>
                </div>
                <p className="text-gray-700 mb-4">
                    This project enables users to compare prices of grocery products across
                    multiple supermarkets, create shopping lists, and find the most affordable
                    options in their area.
                </p>
                <div className="flex flex-wrap gap-3 mt-6">
                    <span className="px-3 py-1 bg-white rounded-full text-sm font-medium text-gray-700">React</span>
                    <span className="px-3 py-1 bg-white rounded-full text-sm font-medium text-gray-700">C#</span>
                    <span className="px-3 py-1 bg-white rounded-full text-sm font-medium text-gray-700">.NET Core</span>
                    <span className="px-3 py-1 bg-white rounded-full text-sm font-medium text-gray-700">TypeScript</span>
                    <span className="px-3 py-1 bg-white rounded-full text-sm font-medium text-gray-700">SQL Server</span>
                    <span className="px-3 py-1 bg-white rounded-full text-sm font-medium text-gray-700">Tailwind CSS</span>
                </div>
            </div>

            {/* Team Section */}
            <div className="mb-12">
                <div className="flex items-center justify-center gap-3 mb-6">
                    <Users className="text-green-600" size={28} />
                    <h2 className="text-2xl font-bold text-gray-900">Team</h2>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    {teamMembers.map((member, index) => (
                        <div key={index} className="bg-white border border-gray-100 rounded-xl p-6 text-center">
                            <h3 className="text-lg font-bold text-gray-900">{member.name}</h3>
                            <p className="text-green-600 text-sm mb-4">{member.role}</p>
                            <div className="flex items-center justify-center gap-4">
                                <a
                                    href={member.github}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="text-gray-500 hover:text-gray-900 transition-colors"
                                >
                                    <Github size={20} />
                                </a>
                                <a
                                    href={member.linkedin}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="text-gray-500 hover:text-blue-600 transition-colors"
                                >
                                    <Linkedin size={20} />
                                </a>
                                <a
                                    href={`mailto:${member.email}`}
                                    className="text-gray-500 hover:text-green-600 transition-colors"
                                >
                                    <Mail size={20} />
                                </a>
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            {/* GitHub Repository */}
            <div className="bg-gray-900 rounded-2xl p-8 text-center">
                <Github className="mx-auto text-white mb-4" size={40} />
                <h3 className="text-xl font-bold text-white mb-2">Open Source</h3>
                <p className="text-gray-400 mb-6">Check out our project on GitHub</p>
                <a
                    href="https://github.com/DogukanOrs/Market-Price-Comparison-System"
                    target="_blank"
                    rel="noopener noreferrer"
                    className="inline-flex items-center gap-2 bg-white text-gray-900 px-6 py-3 rounded-xl font-medium hover:bg-gray-100 transition-colors"
                >
                    <Github size={18} />
                    View Repository
                </a>
            </div>
        </div>
    );
}
