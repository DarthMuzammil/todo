import { motion } from 'motion/react'
import { cn } from '@/lib/utils'

export function StaggerItem({ children, index = 0, className }) {
  return (
    <motion.li
      className={cn(className)}
      initial={{ opacity: 0, y: 12, filter: 'blur(4px)' }}
      animate={{ opacity: 1, y: 0, filter: 'blur(0px)' }}
      transition={{
        duration: 0.4,
        delay: Math.min(index * 0.06, 0.36),
        ease: [0.21, 0.47, 0.32, 0.98],
      }}
      layout
    >
      {children}
    </motion.li>
  )
}
