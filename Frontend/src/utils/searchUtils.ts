/**
 * Search utilities for Turkish character normalization,
 * variant generation, and relevance scoring.
 */

// Turkish character mapping (bidirectional)
const TURKISH_CHAR_MAP: Record<string, string> = {
    'ş': 's', 'Ş': 'S',
    'ğ': 'g', 'Ğ': 'G',
    'ü': 'u', 'Ü': 'U',
    'ö': 'o', 'Ö': 'O',
    'ç': 'c', 'Ç': 'C',
    'ı': 'i', 'İ': 'I',
};

// Reverse mapping (ASCII to Turkish)
const ASCII_TO_TURKISH: Record<string, string[]> = {
    's': ['s', 'ş'], 'S': ['S', 'Ş'],
    'g': ['g', 'ğ'], 'G': ['G', 'Ğ'],
    'u': ['u', 'ü'], 'U': ['U', 'Ü'],
    'o': ['o', 'ö'], 'O': ['O', 'Ö'],
    'c': ['c', 'ç'], 'C': ['C', 'Ç'],
    'i': ['i', 'ı'], 'I': ['I', 'İ'],
};

/**
 * Normalize a string by converting Turkish chars to ASCII and lowercasing.
 */
export function normalizeForSearch(input: string): string {
    if (!input) return '';

    let result = input.trim().toLowerCase();

    for (const [turkish, ascii] of Object.entries(TURKISH_CHAR_MAP)) {
        result = result.replace(new RegExp(turkish, 'g'), ascii);
    }

    // Collapse multiple spaces
    result = result.replace(/\s+/g, ' ');

    return result;
}

/**
 * Generate search variants for a query.
 * E.g., "sut" -> ["sut", "süt"]
 * E.g., "coca cola" -> ["coca cola", "cocacola"]
 */
export function generateSearchVariants(query: string): string[] {
    if (!query || query.trim().length === 0) return [];

    const normalized = query.trim().toLowerCase();
    const variants = new Set<string>();

    // Original query
    variants.add(normalized);

    // ASCII version (süt -> sut)
    variants.add(normalizeForSearch(normalized));

    // Turkish version (sut -> süt) - generate possible Turkish chars
    let turkishVariant = normalized;
    for (const [ascii, turkishChars] of Object.entries(ASCII_TO_TURKISH)) {
        // Only replace if the Turkish version exists
        if (turkishChars.length > 1 && normalized.includes(ascii.toLowerCase())) {
            turkishVariant = turkishVariant.replace(
                new RegExp(ascii.toLowerCase(), 'g'),
                turkishChars[1].toLowerCase()
            );
        }
    }
    if (turkishVariant !== normalized) {
        variants.add(turkishVariant);
    }

    // No-space version (coca cola -> cocacola)
    const noSpaces = normalized.replace(/\s+/g, '');
    if (noSpaces !== normalized && noSpaces.length >= 2) {
        variants.add(noSpaces);
    }

    return Array.from(variants);
}

/**
 * Calculate relevance score for a product against a search query.
 * Higher score = more relevant.
 */
export function calculateRelevance(productName: string, searchQuery: string): number {
    const normalizedProduct = normalizeForSearch(productName);
    const normalizedQuery = normalizeForSearch(searchQuery);

    if (!normalizedProduct || !normalizedQuery) return 0;

    // Exact match (highest)
    if (normalizedProduct === normalizedQuery) {
        return 100;
    }

    // Product starts with query
    if (normalizedProduct.startsWith(normalizedQuery)) {
        return 90;
    }

    // Query is a complete word in product name
    const productWords = normalizedProduct.split(/\s+/);
    const queryWords = normalizedQuery.split(/\s+/);

    let wordMatchScore = 0;
    for (const qWord of queryWords) {
        for (const pWord of productWords) {
            if (pWord === qWord) {
                wordMatchScore += 30;
            } else if (pWord.startsWith(qWord)) {
                wordMatchScore += 20;
            } else if (pWord.includes(qWord)) {
                // Substring match - lower score
                // Check if it's a "bad" substring (like kola in çikolata)
                const idx = pWord.indexOf(qWord);
                if (idx === 0) {
                    wordMatchScore += 15;
                } else {
                    // Middle substring - very low score
                    wordMatchScore += 5;
                }
            }
        }
    }

    // Contains query as substring
    if (normalizedProduct.includes(normalizedQuery)) {
        return Math.max(wordMatchScore, 50);
    }

    return wordMatchScore;
}

/**
 * Sort products by relevance to search query.
 */
export function sortByRelevance<T extends { name: string }>(
    products: T[],
    searchQuery: string
): T[] {
    return [...products].sort((a, b) => {
        const scoreA = calculateRelevance(a.name, searchQuery);
        const scoreB = calculateRelevance(b.name, searchQuery);
        return scoreB - scoreA; // Higher score first
    });
}

/**
 * Filter products by category (if provided).
 * Accepts either a single category name or a list of valid categories (parent + subcategories).
 */
export function filterByCategory<T extends { category?: string }>(
    products: T[],
    categoryName?: string,
    validCategories?: string[]
): T[] {
    if (!categoryName || categoryName === 'All') {
        return products;
    }

    // If validCategories provided (includes subcategories), use them
    if (validCategories && validCategories.length > 0) {
        const normalizedValid = validCategories.map(c => normalizeForSearch(c));
        return products.filter(p => {
            if (!p.category) return false;
            const productCategory = normalizeForSearch(p.category);
            return normalizedValid.some(valid =>
                productCategory === valid ||
                productCategory.includes(valid) ||
                valid.includes(productCategory)
            );
        });
    }

    // Fallback: simple category matching
    const normalizedCategory = normalizeForSearch(categoryName);

    return products.filter(p => {
        if (!p.category) return false;
        const productCategory = normalizeForSearch(p.category);
        return productCategory === normalizedCategory ||
            productCategory.includes(normalizedCategory) ||
            normalizedCategory.includes(productCategory);
    });
}

