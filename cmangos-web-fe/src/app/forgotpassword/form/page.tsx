'use client'

import { useSearchParams, useRouter } from 'next/navigation'
import { useState } from 'react'
import { env } from 'next-runtime-env';

export default function RecoverPassword() {
    const searchParams = useSearchParams()
    const [password, setPassword] = useState('')
    const [password2, setPassword2] = useState('')
    const [passwordError, setPasswordError] = useState('')
    const router = useRouter()
    const NEXT_PUBLIC_FOO = env('NEXT_PUBLIC_FOO');

    const onButtonClick = () => {
        if ('' === password) {
            setPasswordError('Please enter password')
            return
        }

        if ('' === password2) {
            setPasswordError('Please enter password again')
            return
        }

        if (password !== password2) {
            setPasswordError('Passwords do not match')
            return
        }

        fetch(NEXT_PUBLIC_FOO + '/passwordrecovery', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ newPassword: password, token: searchParams.get('token') }),
        })
            .then(async (response) => {
                if (response.status == 400) {
                    setPasswordError('Password recovery failed')
                    throw new Error('Failed to recover password');
                }
                router.push('/forgotpassword/success')
            })
    }

    return (
        <main>
            <div className={'mainContainer'}>
                <div className={'titleContainer'}>
                    <div>Login</div>
                </div>
                <br />
                <label className="errorLabel">{passwordError}</label>  
                <div className={'inputContainer'}>
                    <input
                        value={password}
                        placeholder="Enter password"
                        onChange={(ev) => setPassword(ev.target.value)}
                        className={'inputBox'}
                    />
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input
                        value={password2}
                        placeholder="Enter password again"
                        onChange={(ev) => setPassword2(ev.target.value)}
                        className={'inputBox'}
                    />
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onButtonClick} value={'Log in'} />
                </div>
            </div>
        </main>
    );
}