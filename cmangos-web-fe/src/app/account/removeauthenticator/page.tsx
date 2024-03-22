'use client'

import { useState } from 'react';
import { useRouter } from 'next/navigation'
import { useCookies } from 'react-cookie';
import { env } from 'next-runtime-env';

export default function AuthenticatorRemoval() {
    const [totp, setTotp] = useState('')
    const [totpError, setTotpError] = useState('')
    const [cookies] = useCookies(['access-token'])
    const router = useRouter()
    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');

    const onAuthenticatorRemove = () => {
        if (totp == '') {
            setTotpError("Please enter a totp token from your authenticator app")
            return
        }

        fetch(NEXT_PUBLIC_API + '/removeauthenticator', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer: ' + cookies['access-token'],
            },
            body: JSON.stringify({ token: totp }),
        })
            .then(async (response) => {
                if (response.status == 401) {
                    router.push('/login')
                    throw new Error('Unauthorized');
                }
                else if (response.status == 400) {
                    setTotpError('Token does not match secret')
                    throw new Error('Token does not match secret');
                }
                router.push('/account/removeauthenticator/success')
            })    
    }

    return (
        <div>
            <h1>Authenticator removal</h1>
            <label>{totpError}</label>
            <div className={'inputContainer'}>
                <input
                    value={totp}
                    placeholder="Enter token from authenticator app"
                    onChange={(ev) => setTotp(ev.target.value)}
                    className={'inputBox'}
                />
            </div>
            <br />
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onAuthenticatorRemove} value={'Remove authenticator'} />
            </div>
        </div>
    );
}