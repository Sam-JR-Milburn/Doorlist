/**
 * The paths are a bit ugly, but that's a limitation at compile-time
 */
import localFont from "next/font/local";
export const neueHaas = localFont({
    src: [
        {
            path: "../res/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayXXThin.ttf",
            weight: "100",
            style: "normal"
        },
        {
            path: "../res/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayThin.ttf",
            weight: "200",
            style: "normal"
        },
        {
            path: "../res/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayLight.ttf",
            weight: "300",
            style: "normal"
        },
        {
            path: "../res/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayMedium.ttf",
            weight: "500",
            style: "normal"
        },
        {
            path: "../res/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayBold.ttf",
            weight: "700",
            style: "normal"
        },
        {
            path: "../res/fonts/NeueHaasGroteskDisplay/NeueHaasDisplayBlack.ttf",
            weight: "900",
            style: "normal"
        }
    ],
    variable: "--font-neue-haas-grotesk"
});