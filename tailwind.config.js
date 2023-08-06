module.exports = {
    purge: {
        enabled: true,
        content: ['./Pages/**/*.cshtml', './Views/**/*.chstml'],
    },
    darkMode: false, // or 'media' or 'class'
    theme: {
        extend: {
            colors: {
                teal: {
                    50: '#F0FDFA',
                    100: '#CCFBF1',
                    200: '#99F6E4',
                    300: '#5EEAD4',
                    400: '#2DD4BF',
                    500: '#14B8A6', // This is the color you want to use
                    600: '#0D9488',
                    700: '#0F766E',
                    800: '#115E59',
                    900: '#134E4A',
                },
            },
        },
    },
    variants: {
        extend: {},
    },
    plugins: [],
};