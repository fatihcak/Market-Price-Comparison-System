import { Star } from 'lucide-react';

export default function Testimonials() {
  const testimonials = [
    {
      name: 'John Doe',
      role: 'Customer',
      text: 'I can find the best prices for my products and compare them with other markets. It is a very useful app!',
      rating: 5,
      avatar: '👨'
    },
    {
      name: 'Jane Smith',
      role: 'Customer',
      text: 'I can find the best prices for my products and compare them with other markets. It is a very useful app!',
      rating: 4,
      avatar: '👩'
    },
    {
      name: 'Mark Johnson',
      role: 'Customer',
      text: 'I can find the best prices for my products and compare them with other markets. It is a very useful app!',
      rating: 5,
      avatar: '👨'
    },
    {
      name: 'Jane Smith',
      role: 'Customer',
      text: 'I can find the best prices for my products and compare them with other markets. It is a very useful app!',
      rating: 5,
      avatar: '👩'
    }
  ];

  return (
    <section className="py-20 bg-white">
      <div className="max-w-7xl mx-auto px-4">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">Customers Likes Us</h2>
          <p className="text-lg text-gray-600">Read the stories of happy customers</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {testimonials.map((testimonial, index) => (
            <div
              key={index}
              className="bg-white border border-gray-200 rounded-xl p-6 hover:shadow-lg hover:border-green-200 transition-all duration-300 transform hover:-translate-y-1"
            >
              <div className="flex items-center gap-1 mb-4">
                {[...Array(testimonial.rating)].map((_, i) => (
                  <Star key={i} size={16} className="fill-yellow-400 text-yellow-400" />
                ))}
              </div>

              <p className="text-gray-700 mb-4 text-sm leading-relaxed">"{testimonial.text}"</p>

              <div className="flex items-center gap-3 pt-4 border-t border-gray-100">
                <div className="text-3xl">{testimonial.avatar}</div>
                <div>
                  <p className="font-semibold text-gray-900 text-sm">{testimonial.name}</p>
                  <p className="text-gray-500 text-xs">{testimonial.role}</p>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 mt-20">
          <div className="text-center">
            <div className="text-4xl font-bold text-green-600 mb-2">500K+</div>
            <p className="text-gray-600">Active Users</p>
          </div>
          <div className="text-center">
            <div className="text-4xl font-bold text-green-600 mb-2">2M+</div>
            <p className="text-gray-600">Compared Prices</p>
          </div>
          <div className="text-center">
            <div className="text-4xl font-bold text-green-600 mb-2">1B+</div>
            <p className="text-gray-600">Saved Money</p>
          </div>
        </div>
      </div>
    </section>
  );
}
