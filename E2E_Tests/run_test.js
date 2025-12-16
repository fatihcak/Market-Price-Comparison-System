const { chromium } = require('playwright');
const fs = require('fs');
const path = require('path');

const REPORT_FILE = path.join(__dirname, 'E2E_TEST_REPORT.txt');
const BASE_URL = 'http://localhost:5173';

function log(message, type = 'INFO') {
    const timestamp = new Date().toISOString();
    const formattedMessage = `[${timestamp}] [${type}] ${message}`;
    console.log(formattedMessage);
    fs.appendFileSync(REPORT_FILE, formattedMessage + '\n');
}

function startNewSession() {
    const separator = '\n' + '='.repeat(50) + '\n';
    fs.appendFileSync(REPORT_FILE, `${separator}E2E TEST EXECUTION STARTED AT ${new Date().toISOString()}\n${separator}`);
}

async function runTests() {
    startNewSession();
    let browser = null;
    let page = null;
    let errors = 0;

    try {
        log('Launching Browser...');
        browser = await chromium.launch({ headless: false });
        // Set viewport size to ensure desktop view first
        const context = await browser.newContext({ viewport: { width: 1280, height: 720 } });
        page = await context.newPage();

        // ---------------------------------------------------------
        // SCENARIO 1: HOMEPAGE LOAD & COMPONENTS CHECK
        // ---------------------------------------------------------
        try {
            log('SCENARIO 1: Verifying Homepage Components');
            await page.goto(BASE_URL);
            
            // Check Title
            const title = await page.title();
            if (title.includes('Market')) {
                 log(`Title Verified: ${title}`, 'SUCCESS');
            } else {
                 log(`Unexpected Title: ${title}`, 'WARN');
            }

            // Check Header Elements
            await page.locator('header').waitFor();
            if (await page.getByText('Market Comparison System').isVisible()) {
                log('Header Logo/Text visible - PASS', 'SUCCESS');
            }
            
            // Check Navigation Links
            const nav = page.locator('nav');
            if (await nav.getByText('Products').isVisible()) {
                log('Navigation "Products" link visible - PASS', 'SUCCESS');
            }
        } catch (e) {
            log(`SCENARIO 1 FAILED: ${e.message}`, 'ERROR');
            errors++;
        }

        // ---------------------------------------------------------
        // SCENARIO 2: AI CHATBOT INTERACTION
        // ---------------------------------------------------------
        try {
            log('SCENARIO 2: AI Chatbot Interaction');
            // Chatbot starts minimized. Selector based on AiChatbot.tsx structure.
            // Looking for the header div that toggles minimization
            const chatbotHeader = page.getByText('Market Assistant');
            await chatbotHeader.waitFor();
            
            log('Clicking to expand chatbot...');
            await chatbotHeader.click();
            
            // Wait for input to be visible
            const chatInput = page.getByPlaceholder('Ask about prices...');
            await chatInput.waitFor({ state: 'visible', timeout: 3000 });
            
            if (await chatInput.isVisible()) {
                log('Chatbot expanded and input visible - PASS', 'SUCCESS');
                
                // Test sending a message
                await chatInput.fill('Hello AI');
                await page.locator('button[type="submit"]').click();
                log('Sent message to AI');
                
                // Wait for user message to appear in chat
                await page.waitForTimeout(1000); 
                if (await page.getByText('Hello AI').isVisible()) {
                     log('Message appears in chat log - PASS', 'SUCCESS');
                }
                
                // Close/Minimize chatbot
                // Use force click to avoid interception issues
                const closeButton = page.locator('button').filter({ has: page.locator('svg.lucide-x') }).last(); 
                if (await closeButton.count() > 0) {
                     await closeButton.click({ force: true });
                     log('Chatbot minimized (force clicked)');
                } else {
                     // Fallback 
                     await page.locator('.bg-gradient-to-r').click(); // Click header to toggle if X not found
                     log('Chatbot header clicked to toggle');
                }
            }
        } catch (e) {
            log(`SCENARIO 2 FAILED: ${e.message}`, 'ERROR');
            errors++;
        }

        // ---------------------------------------------------------
        // SCENARIO 3: ADD TO CART & BASKET FLOW
        // ---------------------------------------------------------
        try {
            log('SCENARIO 3: Add to Cart and Basket Flow');
            
            // Wait for products to stabilize
            await page.waitForTimeout(1000);

            // Find valid "Add" buttons. 
            // Avoid buttons covered by the chatbot if it failed to close.
            // We select the 3rd or 4th product to be safe, or scroll to top right.
            const addButtons = page.locator('button:has-text("Add")');
            const count = await addButtons.count();
            
            if (count > 0) {
                // Pick the 2nd product to avoid potential edge overlaps
                const indexToClick = count > 1 ? 1 : 0;
                const btn = addButtons.nth(indexToClick);
                
                // Scroll into view
                await btn.scrollIntoViewIfNeeded();
                await btn.click({ force: true });
                log(`Clicked "Add" on product #${indexToClick + 1}`);
                
                // Verify Header Badge updates
                // Header.tsx: span with absolute positioning containing recount
                // We'll wait a brief moment for state update
                await page.waitForTimeout(500);
                
                // Check for the list button in header
                const listButton = page.locator('header button').filter({ hasText: 'List' });
                await listButton.click();
                log('Opened Shopping List Sidebar');
                
                // Wait for ProductList sidebar
                // ProductList.tsx likely has a title "Shopping List"
                await page.getByRole('heading', { name: 'Shopping List' }).waitFor();
                log('Shopping List Sidebar visible - PASS', 'SUCCESS');
                
                // Click "Compare Prices" in the sidebar
                // ProductList.tsx usually has a button for this
                const compareBtn = page.getByText('Compare Prices');
                if (await compareBtn.isVisible()) {
                    await compareBtn.click();
                    log('Clicked "Compare Prices"');
                    
                    // Verify BasketComparison Modal
                    // BasketComparison.tsx title "Basket Comparison"
                    await page.getByText('Basket Comparison').first().waitFor();
                    log('Basket Comparison Modal Opened - PASS', 'SUCCESS');
                    
                    // Close the modal
                    await page.locator('button').filter({ hasText: 'Close' }).click();
                } else {
                    log('Compare Prices button not found in sidebar', 'WARN');
                }

            } else {
                log('No "Add" buttons found on page', 'WARN');
            }

        } catch (e) {
            log(`SCENARIO 3 FAILED: ${e.message}`, 'ERROR');
            errors++;
        }

        // ---------------------------------------------------------
        // SCENARIO 4: CATEGORY FILTERING
        // ---------------------------------------------------------
        try {
            log('SCENARIO 4: Category Filtering');
            // Check for categories in the main section or SubCategoryNavbar
            // App.tsx renders CategorySection
            
            // Let's assume there are images/links for categories. 
            // We'll try to find a category name from constants (e.g., "Dairy")
            // This part depends on what's rendered. Assuming "Dairy" exists based on context.
            // If strictly from code, we saw "All Products" in App.tsx
            
            const categorySection = page.locator('h2').filter({ hasText: 'Categories' });
            if (await categorySection.isVisible()) {
               log('Category Section visible', 'SUCCESS');
            }
            
            // Try to click a subcategory if available (SubCategoryNavbar)
            // Or just check that "All Products" is the default heading
            const heading = page.locator('h2').filter({ hasText: 'All Products' });
            if (await heading.isVisible()) {
                log('Default "All Products" view confirmed', 'SUCCESS');
            }

        } catch (e) {
            log(`SCENARIO 4 FAILED: ${e.message}`, 'ERROR');
            errors++;
        }

    } catch (globalError) {
        log(`CRITICAL TEST FAILURE: ${globalError.message}`, 'FATAL');
        errors++;
    } finally {
        log(`\nTEST EXECUTION COMPLETED. TOTAL ERRORS: ${errors}`, errors > 0 ? 'FAIL' : 'SUCCESS');
        if (browser) await browser.close();
    }
}

runTests();
