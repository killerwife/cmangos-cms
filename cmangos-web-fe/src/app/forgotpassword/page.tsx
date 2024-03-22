'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { env } from 'next-runtime-env';

export default function ForgotPassword() {
    const [email, setEmail] = useState('')
    const [emailError, setEmailError] = useState('')
    const router = useRouter()
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');

    const onButtonClick = () => {
        if ('' === email) {
            setEmailError('Please enter email for password recovery')
            return
        }

        fetch(NEXT_PUBLIC_API + '/forgotpassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email }),
        }).then(async (response) => {
            if (response.status == 400) {
                setEmailError('Invalid email')
                throw new Error('Failed to send email');
            }
            router.push('/forgotpassword/sent')
        })
    }

    return (
        <main>
            <div className={'mainContainer'}>
                <div className={'titleContainer'}>
                    <div>Forgotten password</div>
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input
                        value={email}
                        placeholder="Enter email"
                        onChange={(ev) => setEmail(ev.target.value)}
                        className={'inputBox'}
                    />
                    <label className="errorLabel">{emailError}</label>
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onButtonClick} value={'Log in'} />
                </div>
            </div>
        </main>
    );
}