# Learning Tailwind

Use cheatsheet to look up Tailwind classes: https://flowbite.com/tools/tailwind-cheat-sheet/

## Breakpoints

Use breakpoints like the following: `sm:w-32`

| Breakpoint prefix | Minimum width | CSS                                  |
| ----------------- | ------------- | ------------------------------------ |
| `sm`              | 640px         | `@media (min-width: 640px { ... })`  |
| `md`              | 768px         | `@media (min-width: 768px { ... })`  |
| `lg`              | 1024px        | `@media (min-width: 1024px { ... })` |
| `xl`              | 1280px        | `@media (min-width: 1280px { ... })` |
| `2xl`             | 1536px        | `@media (min-width: 1536px { ... })` |

## Theme

In `tailwind.config.js` you can write variables that can be referenced in the inline Tailwind.

For example, once you have written a `colors: {"primary": /* "hex code" */} }`, then you can reference it in inline Tailwind such as `bg-primary`.

You can also reference the color in normal css using `theme("colors.primary")`.

## index.css

This is for writing classes, so you don't rewrite styles for elements/components that share styles.

It is normally written with raw CSS, but you can even use Tailwind inside the css by using `@apply`.

The order of the `@tailwind` snippets are important.

If you want to add custom styles for specific HTML elements, add those to Tailwind's Base layer. For example: "<h2>".

Use the components layer for React components. For example: "<Card>, <Button>"

```css
@layer components {
  .card {
    @apply flex items-center justify-center;

    /* raw css for styling this className */
  }
}
```

Use the utilities layer for writing custom CSS utilities that Tailwind doesn't provide.
